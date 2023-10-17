using System.Collections;
using CardLibrary;
using PlayersWebApp.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace PlayersWebApp.Validators;

public class CardDeckValidator
{
    private static JSchema? _jSchema;
    private static readonly int CardsCount = 18;

    private static async Task<IList<CardFromClientDto>?> ReadAsync(JsonReader reader)
    {
        var serializer = new JsonSerializer();
        return await new Task<IList<CardFromClientDto>?>(
            () => serializer.Deserialize<IList<CardFromClientDto>>(reader));
    }
    
    public static CardDeck? ValidateAndReturn(string data, out IList<string>? errMessages)
    {
        if (_jSchema == null)
        {
            var generator = new JSchemaGenerator();
            _jSchema = generator.Generate(typeof(IEnumerable<CardFromClientDto>));
            Console.WriteLine(_jSchema.ToString());
        }
        
        var reader = new JsonTextReader(new StringReader(data));

        var validatingReader = new JSchemaValidatingReader(reader);
        validatingReader.Schema = _jSchema;

        var messages = new List<string>();
        validatingReader.ValidationEventHandler += (o, a) => messages.Add(a.Message);
        
        var serializer = new JsonSerializer();
        var cardDtos = serializer.Deserialize<IEnumerable<CardFromClientDto>>(validatingReader);
        var cardDtosArray = (cardDtos ?? Array.Empty<CardFromClientDto>()).ToArray();
        
        if (cardDtosArray.Length != CardsCount)
        {
            messages.Add($"Amount of cards should be {CardsCount} has {cardDtosArray.Length}");
        }

        if (messages.Count == 0)
        {
            var cards = new List<Card>();
            foreach (var dto in cardDtosArray)
            {
                cards.Add(dto.ToCard());
            }
            errMessages = null;
            return new CardDeck(cards);
        }

        errMessages = messages;
        return null;
    }
}