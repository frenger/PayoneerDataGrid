using PayoneerExercise.Models;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using DataTables.Mvc;
using PayoneerExercise.Utility;

namespace PayoneerExercise.Controllers
{
    class UserJsonData
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public UserJsonData(int userId, string firstName, string lastName, int age)
        {
            this.UserId = userId;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Age = age;
        }
    }

    class UserRequestModel
    {
        public int StartRowIndex { get; set; }
        public int ShownRowsCount { get; set; }
        public string SearchText { get; set; }
        public string OrderColumnName { get; set; }
        public Column.OrderDirection SortOrder { get; set; }

        public UserRequestModel(int startRowIndex, int shownRowsCount, string searchText, string orderColumnName, Column.OrderDirection sortOrder)
        {
            this.StartRowIndex = startRowIndex;
            this.ShownRowsCount = shownRowsCount;
            this.SearchText = searchText;
            this.OrderColumnName = orderColumnName;
            this.SortOrder = sortOrder;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
                
            UserRequestModel item = obj as UserRequestModel;

            return item.StartRowIndex == this.StartRowIndex &&
                item.ShownRowsCount == this.ShownRowsCount &&
                item.SearchText == this.SearchText &&
                item.OrderColumnName == this.OrderColumnName &&
                item.SortOrder == this.SortOrder;
        }

        public override int GetHashCode()
        {
            int hash = 11;
            hash = (hash * 7) + StartRowIndex.GetHashCode();
            hash = (hash * 7) + ShownRowsCount.GetHashCode();
            hash = (hash * 7) + SearchText.GetHashCode();
            hash = (hash * 7) + OrderColumnName.GetHashCode();
            hash = (hash * 7) + SortOrder.GetHashCode();
            return hash;
        }
    }

    public class UserController : Controller
    {

        private ApplicationDbContext _dbContext;
        private static LRUCache<UserRequestModel, DataTablesResponse> _cache = new LRUCache<UserRequestModel, DataTablesResponse>(100);

        public ApplicationDbContext DbContext
        {
            get
            {
                return _dbContext ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                _dbContext = value;
            }
        }


        public UserController()
        {

        }

        public UserController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        private DataTablesResponse GetDataFromDatabase([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IQueryable<User> query = DbContext.Users;
            int recordsTotal = query.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                query = query.Where(p => p.FirstName.StartsWith(value) ||
                                         p.LastName.StartsWith(value)
                                   );
            }
            int recordsFiltered = query.Count();
            #endregion Filtering

            #region Sorting
            // Sorting
            var sortedColumn = requestModel.Columns.GetSortedColumns().First();
            var orderByString = (sortedColumn.Data) + (sortedColumn.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
            query = query.OrderBy(orderByString);
            #endregion Sorting

            #region Paging
            // Paging
            query = query.Skip(requestModel.Start).Take(requestModel.Length);
            #endregion Paging

            var userJsonDataList = query.AsEnumerable().Select(user => new UserJsonData(user.UserId, user.FirstName, user.LastName, user.Age)).ToList();
            return new DataTablesResponse(requestModel.Draw, userJsonDataList, recordsFiltered, recordsTotal);
        }


        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Get([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            var userRequestModel = new UserRequestModel(requestModel.Start, 
                requestModel.Length, 
                requestModel.Search.Value, 
                requestModel.Columns.GetSortedColumns().First().Data,
                requestModel.Columns.GetSortedColumns().First().SortDirection);
            var datatablesResponse = _cache.Get(userRequestModel);

            if (datatablesResponse == null)
            {
                datatablesResponse = GetDataFromDatabase(requestModel);
                _cache.Add(userRequestModel, datatablesResponse);
            }
            else
            {
                //Draw property has to be renewed each Get
                datatablesResponse = new DataTablesResponse(requestModel.Draw, datatablesResponse.data, datatablesResponse.recordsFiltered, datatablesResponse.recordsTotal);
            }

            return Json(datatablesResponse, JsonRequestBehavior.AllowGet);
        }

    }
}
