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

        public async Task<List<Subject>> QueryRndTopAsync(int top)
        {
            return await _iSubjectRepository.QueryRndTopAsync(top);
        }

        public async Task<List<Subject>> QueryTopAsync(int top)
        {
            return await this._iSubjectRepository.QueryTopAsync(top);
        }
    }
}
