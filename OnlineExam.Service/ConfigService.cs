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
    public class ConfigService:BaseService<Config>,IConfigService
    {
        private readonly IConfigRepository _iConfigRepository;

        public ConfigService(IConfigRepository iConfigRepository)
        {
            this._iConfigRepository = iConfigRepository;
            base._iBaseRepository = iConfigRepository;
        }
    }
}
