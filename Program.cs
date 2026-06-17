using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Diagnostics;

DotNetEnv.Env.Load();
var apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");

var client = new OpenAIClient(
    new ApiKeyCredential(apiKey ?? throw new InvalidOperationException("GROQ_API_KEY not set in environment variables.")),  
    new OpenAIClientOptions
    {
        Endpoint = new Uri("https://api.groq.com/openai/v1")
    }
);

var chat = client.GetChatClient("llama-3.3-70b-versatile");
var historique = new List<ChatMessage>();

historique.Add(new SystemChatMessage("You are a helpful assistant that always responds in English."));

Console.WriteLine("Chatbot ready! Type your message (or 'exit' to quit):");

while (true)
{
    Console.Write("You: ");
    var message = Console.ReadLine();

    if (message == "exit") break;

    // Étape 1 : extraire les mots-clés via l'IA
    var motsCles = await ExtraireMotsCles(client, message);

    // Étape 2 : afficher la question avec mots-clés en couleur
    //AfficherAvecCouleurs(message, motsCles);

    // Étape 3 : envoyer la vraie question et mesurer le temps
    historique.Add(new UserChatMessage(message));

    var chrono = Stopwatch.StartNew();
    var reponse = await chat.CompleteChatAsync(historique);
    chrono.Stop();

    var texte = reponse.Value.Content[0].Text;
    historique.Add(new AssistantChatMessage(texte));

    SauvegarderConversation(message, texte, chrono.ElapsedMilliseconds);

    //Console.WriteLine($"AI: {texte}");
    AfficherAvecCouleurs($"AI: {texte}", motsCles);
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine($"⏱ Response time: {chrono.ElapsedMilliseconds} ms");
    Console.ResetColor();
}

async Task<List<string>> ExtraireMotsCles(OpenAIClient client, string? question)
{
    if (string.IsNullOrWhiteSpace(question))
    {
        return new List<string>();
    }

    var chatMotsCles = client.GetChatClient("llama-3.3-70b-versatile");

    var prompt = $@"Extract the 3 to 5 most important keywords from this sentence.
Return ONLY the keywords separated by commas, nothing else.
No explanation, no punctuation, just the words.
Sentence: {question}";

    var resultat = await chatMotsCles.CompleteChatAsync(
        new List<ChatMessage> { new UserChatMessage(prompt) }
    );

    var mots = resultat.Value.Content[0].Text
        .Split(',')
        .Select(m => m.Trim().ToLower())
        .ToList();

    return mots;
}

void AfficherAvecCouleurs(string? texte, List<string> motsCles)
{
    if (string.IsNullOrWhiteSpace(texte))
    {
        return;
    }

    var mots = texte.Split(' ');

    
    foreach (var mot in mots)
    {
        var motPropre = mot.ToLower().Trim('.', '?', '!', ',');

        if (motsCles.Contains(motPropre))
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(mot + " ");
            Console.ResetColor();
        }
        else
        {
            Console.Write(mot + " ");
        }
    }
    Console.WriteLine();
}

void SauvegarderConversation(string? question, string reponse, long temps)
{
    var ligne = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n" +
                $"You: {question}\n" +
                $"AI: {reponse}\n" +
                $"Response time: {temps} ms\n" +
                $"---\n";

    File.AppendAllText("conversation.txt", ligne);
}