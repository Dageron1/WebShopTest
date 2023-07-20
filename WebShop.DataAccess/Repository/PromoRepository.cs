using WebShop.DataAccess.Repository.IRepository;
using WebShop.DataAcess.Data;
using WebShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.DataAccess.Repository
{
    public class PromoRepository : Repository<PromoCode>, IPromoRepository
    {
        private ApplicationDbContext _db;
        public PromoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        

        public void Update(PromoCode obj)
        {
            var objFromDb = _db.PromoCodes.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.Discount = obj.Discount;
                
                //if (obj.ImageUrl != null)
                //{
                //    objFromDb.ImageUrl = obj.ImageUrl;
                //}
            }
        }
    }
}
