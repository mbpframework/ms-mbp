using WuhanIns.Nitrogen.Ddd.Application.Dto;
using System;

namespace Ins.Sample.Service.Dto
{
    public class DemoInputDto : DtoBase
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string CurState { get; set; }

        public DateTime Concurrencystamp { get; set; }
    }
}
