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
    public class SubjectService:BaseService<Subject>,ISubjectService
    {
        private readonly ISubjectRepository _iSubjectRepository;

        public SubjectService(ISubjectRepository iSubjectRepository)
        {
            this._iSubjectRepository = iSubjectRepository;
            base._iBaseRepository = iSubjectRepository;
        }
    }
}
