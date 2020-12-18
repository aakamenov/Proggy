using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Proggy.Core;

namespace Proggy.Infrastructure.Converters.Json
{
    public class BarInfoConverter : JsonConverter<BarInfo[]>
    {
        public override BarInfo[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = new List<BarInfo>();

            reader.Read(); //Start object

            while(reader.Read())
            {
                byte beats = 4;
                byte noteLength = 4; 
                short tempo = 120;

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                            break;

                        if (reader.TokenType == JsonTokenType.PropertyName)
                        {
                            switch (reader.GetString())
                            {
                                case "beats":
                                    reader.Read();
                                    beats = reader.GetByte();
                                    break;
                                case "noteLength":
                                    reader.Read();
                                    noteLength = reader.GetByte();
                                    break;
                                case "tempo":
                                    reader.Read();
                                    tempo = reader.GetInt16();
                                    break;
                            }
                        }
                    }

                    result.Add(new BarInfo(tempo, beats, noteLength));
                }
            }

            return result.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, BarInfo[] value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteStartArray("bars");

            foreach (var bar in value)
            {
                writer.WriteStartObject();

                writer.WriteNumber("beats", bar.Beats);
                writer.WriteNumber("noteLength", bar.NoteLength);
                writer.WriteNumber("tempo", bar.Tempo);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}
