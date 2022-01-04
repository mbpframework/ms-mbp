using Mbp.Ddd.Application.Dto;

namespace Mbp.Framework.Application.Contracts.Dto
{
    public class SampleOutputDto: DtoBase
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string CurState { get; set; }

        public string Concurrencystamp { get; set; }
    }
}
