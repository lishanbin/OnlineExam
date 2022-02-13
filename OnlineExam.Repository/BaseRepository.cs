using OnlineExam.IRepository;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExam.Repository
{
    public class BaseRepository<TEntity> : SimpleClient<TEntity>,IBaseRepository<TEntity> where TEntity : class, new()
    {
        public BaseRepository(ISqlSugarClient context=null):base(context)
        {
            base.Context = DbScoped.Sugar;
        }
        public Task<bool> CreateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FindAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> func)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> QueryAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> func)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> QueryAsync(int page, int size, RefAsync<int> total)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> func, int page, int size, RefAsync<int> total)
        {
            throw new NotImplementedException();
        }
    }
}
