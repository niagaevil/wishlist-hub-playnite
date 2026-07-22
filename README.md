# Wishlist Hub — Playnite extension

Extensão **Generic** para o [Playnite](https://playnite.link/) que envia jogos da sua biblioteca para o [Wishlist Hub](https://wishlist-hub.paz.poa.br).

## Recursos (v1.1)

- Envio manual: menu **Extensions → Wishlist Hub** ou menu do jogo
- **Sync automática** ao adicionar jogos (debounce ~8s + fila persistente); processa também após `OnLibraryUpdated`
- Tags locais opcionais: `[WishlistHub] Synced` / `Miss` / `Ignored` / `Error`
- Settings: token, Base URL, bibliotecas, toggles de auto-sync / tags / notificações
- Merge **aditivo** no Hub — não remove OAuth, cookies, navegador nem Ubisoft

## Instalação

### Add-on browser (quando o PR for mergeado)

1. Playnite → **Add-ons** → Browse → Generic → **Wishlist Hub**
2. Ou abra: https://playnite.link/addons.html#WishlistHub_Playnite  
3. Ou URI: `playnite://playnite/installaddon/WishlistHub_Playnite`

Enquanto isso, use sideload (abaixo). Ver `ci/ADDON_BROWSER.md`.

### Sideload (Releases)

1. Baixe o `.pext` em [Releases](https://github.com/niagaevil/wishlist-hub-playnite/releases)
2. Playnite → **Add-ons** → **Install from file**

## Configuração

1. No Hub: [Ferramentas → Playnite](https://wishlist-hub.paz.poa.br/ferramentas#playnite) → **Gerar token**
2. Playnite → Add-ons → Extension settings → **Wishlist Hub** → colar token
3. (Opcional) desmarque Steam/GOG se já sincroniza nativo no Hub
4. Deixe **Enviar jogos novos automaticamente** ligado se quiser sync contínua

## Compilar / empacotar (Windows)

```bat
copy Directory.Build.props.example Directory.Build.props
REM ajuste PlayniteDir
msbuild WishlistHub.sln /p:Configuration=Release
ci\pack.bat
```

Anexe o `.pext` na Release `v1.1.0` com o nome `WishlistHub_Playnite_1_1_0.pext` (URL já referenciada em `ci/installer_manifest.yaml`).

## API do Hub

`POST {BaseUrl}/api/playnite/collection/import`

```json
{ "version": "v1", "token": "...", "data": "<json-array-string>" }
```

## Licença

MIT
