using HtmlAgilityPack; 
using MongoDB.Bson; 
using MongoDB.Driver; 
using System; 
using System.Collections.Generic; 
using System.Net.Http; 
using System.Threading.Tasks; 

class Program
{
    static async Task Main(string[] args)
    {
        // Criação do contexto do MongoDB
        var dbContext = new MongoDbContext();
        var alimentosCollection = dbContext.GetAlimentosCollection(); // Obtém a coleção de alimentos

        try
        {
            int totalPaginas = 10; // Aqui definimos o número total de páginas a serem acessadas
            for (int pagina = 1; pagina <= totalPaginas; pagina++)
            {
                // Monta a URL da página a ser acessada
                string url = $"https://www.tbca.net.br/base-dados/composicao_estatistica.php?pagina={pagina}&atuald=1";
                Console.WriteLine($"Acessando a página {pagina}");

                using var httpClient = new HttpClient(); // Cria um cliente HTTP para realizar requisições
                var html = await httpClient.GetStringAsync(url); // Realiza a requisição e obtém o HTML da página

                var htmlDocument = new HtmlDocument(); // Cria um documento HTML para manipulação
                htmlDocument.LoadHtml(html); // Carrega o HTML obtido

                // Seleciona a tabela da página
                var table = htmlDocument.DocumentNode.SelectSingleNode("//table");

                if (table != null) // Verifica se a tabela foi encontrada
                {
                    // Seleciona todas as linhas da tabela
                    var rows = table.SelectNodes(".//tr");
                    foreach (var row in rows) // Itera sobre cada linha da tabela
                    {
                        var cells = row.SelectNodes("td"); // Seleciona as células da linha
                        if (cells != null) // Verifica se as células foram encontradas
                        {
                            // Extrai os dados de cada célula e remove espaços em branco
                            string codigo = cells[0].InnerText.Trim();
                            string nome = cells[1].InnerText.Trim();
                            string nomeCientifico = cells[2].InnerText.Trim();
                            string grupo = cells[3].InnerText.Trim();

                            // Obter a descrição nutricional do alimento
                            var descricao = await GetDescricaoAlimento(httpClient, codigo);

                            // Cria um documento BSON para armazenar os dados do alimento
                            var alimento = new BsonDocument
                            {
                                { "codigo", codigo },
                                { "nome", nome },
                                { "nome_cientifico", nomeCientifico },
                                { "grupo", grupo },
                                { "descricao", descricao }
                            };

                            // Insere o documento no banco de dados do MongoDB
                            await alimentosCollection.InsertOneAsync(alimento);
                            Console.WriteLine($"Alimento {nome} inserido no MongoDB."); 
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Tabela não encontrada na página " + pagina); 
                }
            }

            Console.WriteLine("Web scraping e inserção finalizados."); 
        }
        catch (Exception e) 
        {
            Console.WriteLine($"Erro ao acessar a URL: {e.Message}"); 
        }
    }

    // Método assíncrono para obter a descrição nutricional do alimento
    static async Task<string> GetDescricaoAlimento(HttpClient httpClient, string codigo)
    {
        try
        {
            // Monta a URL para obter a descrição do alimento
            string descricaoUrl = $"https://www.tbca.net.br/base-dados/int_composicao_estatistica.php?cod_produto={codigo}";
            var htmlDescricao = await httpClient.GetStringAsync(descricaoUrl); // Realiza a requisição

            var htmlDocument = new HtmlDocument(); // Cria um novo documento HTML
            htmlDocument.LoadHtml(htmlDescricao); // Carrega o HTML da descrição

            // Seleciona o corpo da tabela que contém as informações nutricionais
            var tabelaNutricao = htmlDocument.DocumentNode.SelectSingleNode("//tbody");

            if (tabelaNutricao != null) 
            {
                var descricao = new List<BsonDocument>(); // Lista para armazenar os dados nutricionais
                var rows = tabelaNutricao.SelectNodes(".//tr"); // Seleciona as linhas da tabela

                foreach (var row in rows) 
                {
                    var cells = row.SelectNodes("td"); // Seleciona as células da linha
                    if (cells != null && cells.Count >= 3) // Verifica se as células foram encontradas e se há pelo menos 3
                    {
                        // Extrai os dados de cada célula e remove espaços em branco
                        string nutriente = cells[0].InnerText.Trim();
                        string unidade = cells[1].InnerText.Trim();
                        string valor = cells[2].InnerText.Trim();

                        // Adiciona os dados nutricionais a um documento BSON
                        var dadosNutricionais = new BsonDocument
                        {
                            { "nutriente", nutriente },
                            { "unidade", unidade },
                            { "valor", valor }
                        };

                        descricao.Add(dadosNutricionais); // Adiciona os dados nutricionais à lista
                    }
                }

                return descricao.ToJson(); // Converte a lista de descrições para JSON e retorna
            }

            return "Descrição não encontrada"; 
        }
        catch (Exception e) 
        {
            Console.WriteLine($"Erro ao acessar a descrição do alimento {codigo}: {e.Message}"); 
            return "Erro ao buscar descrição"; 
        }
    }
}
