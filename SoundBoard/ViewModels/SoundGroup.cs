using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundBoard.ViewModels
{
    public class SoundGroup
    {
        public string Title { get; set; }
        public List<SoundData> Items { get; set; }
          
        public SoundGroup()
        {
            Items = new List<SoundData>();
        }
    }
}
