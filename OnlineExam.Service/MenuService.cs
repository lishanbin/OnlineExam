using OnlineExam.IRepository;
using OnlineExam.IService;
using OnlineExam.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExam.Service
{
    public class MenuService:BaseService<Menu>,IMenuService
    {
        private readonly IMenuRepository _iMenuRepository;

        public MenuService(IMenuRepository iMenuRepository)
        {
            this._iMenuRepository = iMenuRepository;
            base._iBaseRepository = iMenuRepository;
        }
    }
}
