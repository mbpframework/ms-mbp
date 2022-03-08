using Ins.Sample.DataAccess;
using Ins.Sample.DataAccess.Do;
using Ins.Sample.Service.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using WuhanIns.Nitrogen.Configuration;
using WuhanIns.Nitrogen.Ddd.Application.ObjectMapper;
using WuhanIns.Nitrogen.Ddd.Domain.Repository;
using WuhanIns.Nitrogen.EventBus;
using WuhanIns.Nitrogen.Web.Convention;

namespace Ins.Sample.Service.DemoService
{
    [Route("Demo")]
    public class DemoAppService : IAppService, INgSubscribe
    {
        // 配置
        private readonly IOptions<ProductConfig> _options = null;
        // 对象映射
        private readonly INgObjectMapper<InsDemoServiceModule> _objectMapper = null;
        // 用户信息仓储
        private readonly INgRepository<DemoEntity, DefaultDbContext, Guid> _ngUserRepository = null;

        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="options"></param>
        public DemoAppService(IOptions<ProductConfig> options,
            INgObjectMapper<InsDemoServiceModule> objectMapper,
            INgRepository<DemoEntity, DefaultDbContext, Guid> ngUserRepository)
        {
            _options = options;
            _objectMapper = objectMapper;
            _ngUserRepository = ngUserRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetBlogs/{id}")]
        public virtual string GetBlogs(int id)
        {
            return $"GetBlogs:{id}";
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="userDto"></param>
        [HttpPost("AddUser")]
        public virtual int AddUser(DemoInputDto userDto)
        {
            var entity = _objectMapper.Map<DemoInputDto, DemoEntity>(userDto);

            _ngUserRepository.DbSet.Add(entity);

            return _ngUserRepository.DbContext.SaveChanges();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="userDto"></param>
        [HttpPost("UpdateUser")]
        public virtual int UpdateUser(DemoInputDto userDto)
        {
            var entity = _ngUserRepository.Get(userDto.Id);

            entity.CODE = userDto.Code;
            entity.CUR_STATE = userDto.CurState;
            entity.CONCURRENCYSTAMP = userDto.Concurrencystamp;
            entity.NAME = userDto.Name;

            _ngUserRepository.DbSet.Attach(entity);
            _ngUserRepository.DbSet.Update(entity);

            return _ngUserRepository.DbContext.SaveChanges();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        [HttpPost("DeleteUser")]
        public virtual int DeleteUser(Guid id)
        {
            var entity = _ngUserRepository.Get(id);

            _ngUserRepository.DbSet.Remove(entity);

            return _ngUserRepository.DbContext.SaveChanges();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("GetUser/{id}")]
        public virtual DemoOutputDto GetUser(Guid id)
        {
            var entity = _ngUserRepository.Get(id);

            return _objectMapper.Map<DemoEntity, DemoOutputDto>(entity);
        }
    }
}
