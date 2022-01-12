using MinecraftServerHandler.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerHandler.Utils
{
    public class ColorBuilder
    {
        public List<ColorTag> ColorTags { get; private set; }
        public ColorBuilder(List<ColorTag> _colorTags)
        {
            ColorTags = _colorTags;
        }

        public string ColorFormat(string text)
        {
            string result = "";
            List<string> EndTags = new List<string>();
            for (int i = 0; i < text.Length; ++i)
            {
                if (i < text.Length && (i + 1) < text.Length)
                {
                    if (text[i] == '§')
                    {
                        var cTag = ColorTags.FirstOrDefault(p => p.ColorType == ("§" + text[i + 1]));
                        if (cTag != null)
                        {
                            result += cTag.StartTag;
                            i += 1;
                            EndTags.Add(cTag.EndTag);
                            continue;
                        }

                    }
                }
                result += text[i];
            }
            foreach (string end_tag in EndTags)
            {
                result += end_tag;
            }

            return result;
        }
    }
}
