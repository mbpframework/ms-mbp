using Mbp.Ddd.Application.Dto;
using System;

namespace Mbp.Framework.Application.Contracts.Dto
{
    public class SampleInputDto : DtoBase
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string CurState { get; set; }
    }
}
