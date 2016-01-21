namespace Effort.Extra.Tests
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class FellaWithAttribute
    {
        [Column("Alias")]
        public string Name { get; set; }
    }
}