using Mbp.Ddd.Application.ObjectMapper;
using Mbp.Ddd.Domain.Repository;
using Mbp.EventBus;
using Mbp.Utils;
using Mbp.AspNetCore.Api;
using Mbp.AspNetCore.Convention;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Res.DataAccess;
using Res.DataAccess.Do;
using Res.Service.Dto;

namespace Res.Service.SampleService
{
    [MbpApi(Module = "Sample")]
    public class SampleAppService : IAppService, IMbpSubscribe
    {
        // 配置
        private readonly IOptions<ProductConfig> _options = null;
        // 对象映射
        private readonly INgObjectMapper<InsSampleServiceModule> _objectMapper = null;
        // 用户信息仓储
        private readonly INgRepository<SampleEntity, DefaultDbContext, string> _ngUserRepository = null;

        private readonly INgRepository<OracleEntity, DefaultDbContext, string> _ngOracleEntityRepository = null;

        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="options"></param>
        /// <param name="ngOracleEntityRepository"></param>
        public SampleAppService(IOptions<ProductConfig> options,
            INgObjectMapper<InsSampleServiceModule> objectMapper,
            INgRepository<SampleEntity, DefaultDbContext, string> ngUserRepository,
            INgRepository<OracleEntity, DefaultDbContext, string> ngOracleEntityRepository)
        {
            _options = options;
            _objectMapper = objectMapper;
            _ngUserRepository = ngUserRepository;
            _ngOracleEntityRepository = ngOracleEntityRepository;
        }

        [HttpGet("GetBlogs/{id}")]
        public virtual string GetBlogs(int id)
        {
            return $"GetBlogs:{id}-----{_options.Value.ProductName}";
        }

        [HttpPost("AddUser")]
        public virtual int AddUser(SampleInputDto userDto)
        {
            var entity = _objectMapper.Map<SampleInputDto, SampleEntity>(userDto);

            _ngUserRepository.DbSet.Add(entity);

            return _ngUserRepository.DbContext.SaveChanges();
        }

        [HttpPost("UpdateUser")]
        public virtual int UpdateUser(SampleInputDto userDto)
        {
            var entity = _ngUserRepository.Get(userDto.Id);
            var entityTo = _objectMapper.Map<SampleInputDto, SampleEntity>(userDto);

            EntityUtil.EntityCopy(entityTo, entity);

            _ngUserRepository.DbSet.Attach(entity);
            _ngUserRepository.DbSet.Update(entity);

            return _ngUserRepository.DbContext.SaveChanges();
        }

        [HttpPost("DeleteUser")]
        public virtual int DeleteUser(string id)
        {
            var entity = _ngUserRepository.Get(id);

            _ngUserRepository.DbSet.Remove(entity);

            return _ngUserRepository.DbContext.SaveChanges();
        }

        [HttpGet("GetOracle/{id}")]
        public virtual OracleEntity GetOracle(string id)
        {
            return _ngOracleEntityRepository.Get(id);
        }

        [HttpPost("AddOracle")]
        public virtual int AddOracle(OracleEntity oracleEntity)
        {
            _ngOracleEntityRepository.DbSet.Add(oracleEntity);

            return _ngOracleEntityRepository.DbContext.SaveChanges();
        }

        [HttpPost("UpdateOracle/{id}")]
        public virtual int UpdateOracle(string id, OracleEntity oracleEntity)
        {
            var entity = _ngOracleEntityRepository.Get(id);


            entity.NAME = oracleEntity.NAME;
            entity.CODE = oracleEntity.CODE;

            _ngOracleEntityRepository.DbSet.Attach(entity);
            _ngOracleEntityRepository.DbSet.Update(entity);


            return _ngOracleEntityRepository.DbContext.SaveChanges();
        }

        [HttpPost("DeleteOracle/{id}")]
        public virtual int DeleteOracle(string id)
        {
            var entity = _ngOracleEntityRepository.Get(id);



            _ngOracleEntityRepository.DbSet.Remove(entity);


            return _ngOracleEntityRepository.DbContext.SaveChanges();
        }
    }
}
