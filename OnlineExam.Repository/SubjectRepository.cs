using OnlineExam.IRepository;
using OnlineExam.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExam.Repository
{
    public class SubjectRepository : BaseRepository<Subject>, ISubjectRepository
    {
        public async Task<List<Subject>> QueryRndTopAsync(int top)
        {            
            return await base.Context.SqlQueryable<Subject>("select top (@top) * from subject order by newid()").AddParameters(new SugarParameter[]
            {
                new SugarParameter("@top",top)
            }).ToListAsync();
        }

        public async Task<List<Subject>> QueryTopAsync(int top)
        {
            return await base.Context.Queryable<Subject>().OrderBy(s=>s.Id,SqlSugar.OrderByType.Desc).Take(top).ToListAsync();
        }
    }
}
