using WuhanIns.Nitrogen.Ddd.Application.Dto;

namespace Ins.Sample.Service.Dto
{
    public class DemoOutputDto: DtoBase
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string CurState { get; set; }

        public string Concurrencystamp { get; set; }
    }
}
