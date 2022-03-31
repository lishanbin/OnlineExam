using OnlineExam.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExam.IRepository
{
    public interface ISubjectRepository:IBaseRepository<Subject>
    {
        Task<List<Subject>> QueryTopAsync(int top);
        Task<List<Subject>> QueryRndTopAsync(int top);
    }
}
