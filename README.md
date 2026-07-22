# Wishlist Hub — Playnite extension

Extensão Generic para o [Playnite](https://playnite.link/) que envia jogos da sua biblioteca para o [Wishlist Hub](https://wishlist-hub.paz.poa.br).

Inspirada no fluxo do [Playnite.GGDeals](https://github.com/SparrowBrain/Playnite.GGDeals), mas apontando para a API do Hub.

## O que faz

- Lê jogos das bibliotecas configuradas no Playnite (Steam, Epic, GOG, Ubisoft, EA, Battle.net, Xbox, Amazon, etc.)
- Envia em lote para `POST /api/playnite/collection/import`
- O Hub faz **merge aditivo** — não remove jogos já importados por OAuth, cookies, navegador ou Ubisoft

## Requisitos

1. Conta no Wishlist Hub (login por e-mail)
2. Playnite Desktop (extensões Generic)
3. Token gerado em **Ferramentas → Importar via Playnite**

## Instalação (sideload)

1. Baixe o `.pext` / zip da [página de Releases](https://github.com/niagaevil/wishlist-hub-playnite/releases) (quando publicada) ou compile localmente.
2. No Playnite: menu → **Add-ons** → **Install from file** → selecione o pacote.
3. Reinicie o Playnite se pedido.

## Configuração

1. No Hub: [Ferramentas → Playnite](https://wishlist-hub.paz.poa.br/ferramentas#playnite) → **Gerar token** → copiar.
2. No Playnite: **Add-ons** → Extension settings → **Generic** → **Wishlist Hub**.
3. Cole o token. Base URL padrão: `https://wishlist-hub.paz.poa.br` (ajuste só se usar outro host).
4. Marque as bibliotecas que deseja enviar.
5. Salve.

## Enviar jogos

Menu Playnite → **Extensions** → **Wishlist Hub** → **Enviar jogos ao Wishlist Hub**.

O Hub responde por jogo: `Added`, `Skipped`, `Miss`, `Ignored` ou `Error`.

## Compilar

Requer .NET Framework 4.6.2+ e referência ao Playnite SDK (mesmo padrão das extensões Generic oficiais).

```bash
# Com Visual Studio / MSBuild apontando para o SDK do Playnite instalado
msbuild WishlistHub.sln /p:Configuration=Release
```

Empacote `extension.yaml`, `WishlistHub.dll` e dependências num `.zip` renomeado para `.pext`.

## Contrato da API (v1)

```json
{
  "version": "v1",
  "token": "<token do Hub>",
  "data": "<json-string de array de games>"
}
```

Cada game: `Id`, `GameId`, `Name`, `Links`, `Source`, `ReleaseYear`, `gg_launcher`.

## Licença

MIT
