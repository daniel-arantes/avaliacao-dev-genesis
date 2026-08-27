# Avaliação CDB

Simulador de CDB. Recebe um valor positivo e um prazo inteiro maior que um mês, aplica capitalização mensal e apresenta os resultados bruto e líquido após imposto de renda.

## Tecnologias Usadas

- .NET 10 LTS, ASP.NET Core Controllers e Swagger UI
- Angular 22.1 e Node.js 22.22.3 ou superior compatível
- xUnit, Coverlet e GitHub Actions

## Regras de cálculo

- O CDI fixo é 0,9% e o banco paga 108% do CDI. A taxa mensal efetiva é `0,009 × 1,08 = 0,00972`.
- O imposto incide somente sobre o rendimento: 22,5% até 6 meses; 20% até 12; 17,5% até 24; e 15% acima de 24. Os limites são inclusivos.

```text
valorBruto = valorInicial × (1 + 0,00972) ^ meses
rendimento = valorBruto - valorInicial
imposto = rendimento × alíquota
valorLiquido = valorBruto - imposto
```

## Performance, conceitos e códigos de resposta

- A capitalização implementa exponenciação por quadrados com complexidade `O(log meses)` e memória adicional `O(1)`, evitando `Math.Pow` que converte para `double` e pode perder precisão, mantendo performance.
- Como o contrato não impõe um limite máximo arbitrário para valor ou prazo, combinações extremas podem ultrapassar a capacidade numérica de `decimal`. Essa condição é tratada globalmente pela API e retorna HTTP `422 Unprocessable Entity`, em vez de ser tratado como uma falha interna.
- Entradas incompletas, malformadas ou fora das regras de negócio retornam HTTP `400 Bad Request`.
- Testes unitários e de integração cobrem casos de sucesso, falha e limites, incluindo parâmetros, valores e prazos extremos.

## Estrutura

```text
src/AvaliacaoCdb.Domain            cálculo e política tributária
src/AvaliacaoCdb.Api               contrato HTTP e composição
src/AvaliacaoCdb.Web               interface (Angular)
tests/AvaliacaoCdb.Domain.Tests    testes unitários para o domínio
tests/AvaliacaoCdb.Api.Tests       testes de integração da API
```

## Execução

API:

```powershell
dotnet run --project src/AvaliacaoCdb.Api
```

Frontend:

```powershell
cd src/AvaliacaoCdb.Web
npm ci
npm start
```

Acesse `http://localhost:4200` para o frontend. O proxy encaminha `/api` para `http://localhost:5163`.

Para utilizar e testar somente a API, sem executar o frontend, acesse o Swagger UI:

```text
http://localhost:5163/swagger
```

O documento OpenAPI em JSON fica em `http://localhost:5163/swagger/v1/swagger.json`. A raiz da API redireciona para o Swagger.

## Contrato

`POST /api/investments/cdb/calculate`

```json
{"initialValue": 1000.00, "months": 12}
```

A resposta inclui valor inicial, prazo, valor bruto, rendimento, alíquota, imposto e valor líquido. Entradas inválidas retornam HTTP 400 com `ProblemDetails`.

## Testes e cobertura

```powershell
dotnet test AvaliacaoCdb.slnx --configuration Release
```

Para medir exclusivamente a camada lógica e aplicar a barreira mínima de 90%:

```powershell
dotnet test tests/AvaliacaoCdb.Domain.Tests/AvaliacaoCdb.Domain.Tests.csproj --configuration Release /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=../../coverage/domain/ /p:Include="[AvaliacaoCdb.Domain]*" /p:Threshold=90 /p:ThresholdType=line /p:ThresholdStat=total
```

O relatório fica em `coverage/domain/coverage.cobertura.xml`. O comando e o CI falham abaixo de 90% de linhas em `AvaliacaoCdb.Domain`.

Para testar e compilar o Angular:

```powershell
cd src/AvaliacaoCdb.Web
npm test
npm run build
npm run lint
```

O workflow `.github/workflows/ci.yml` repete builds, testes, análise estática (Sonar para .NET e Angular ESLint) e a verificação de cobertura a cada push ou pull request. 
