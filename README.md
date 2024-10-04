# Projeto de Web Scraping para Coleta de Dados Nutricionais

Este projeto utiliza C# para realizar web scraping de informações nutricionais de alimentos a partir do site TBCA e armazena os dados coletados em um banco de dados MongoDB

## Descrição

O código realiza as seguintes tarefas:

1. Acessa uma série de páginas no site TBCA, onde estão listados alimentos e suas informações nutricionais
2. Extrai dados como código, nome, nome científico e grupo dos alimentos
3. Faz uma requisição adicional para coletar a descrição nutricional detalhada de cada alimento
4. Armazena os dados em um banco de dados MongoDB para posterior consulta e análise

## Tecnologias Utilizadas

- **C#**: Linguagem de programação utilizada para desenvolver o projeto
- **HtmlAgilityPack**: Biblioteca para análise e manipulação de HTML
- **MongoDB**: Banco de dados NoSQL utilizado para armazenar os dados coletados
- **HttpClient**: Classe do .NET para realizar requisições HTTP assíncronas

## Pré-requisitos

Antes de executar o projeto, certifique-se de ter o seguinte instalado:

- [.NET SDK](https://dotnet.microsoft.com/download/dotnet) (versão 6.0 ou superior)
- MongoDB (local ou em nuvem) configurado e em execução
- Pacotes NuGet: HtmlAgilityPack e MongoDB.Driver

## Instalação

1. Clone o repositório:

   ```bash
   git clone https://github.com/Allanvvaz/WebScraperProject.git
   cd WebScrapperProject
