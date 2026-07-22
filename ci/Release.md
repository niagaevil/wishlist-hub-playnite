## Wishlist Hub Playnite v1.1.1

### Destaques
- Fix do import: body JSON em camelCase (antes o Hub via `Token` PascalCase como ausente)
- Trim automático do token e Base URL

### Instalação
1. Baixe `WishlistHub_Playnite_1_1_1.pext` (build no Windows com `ci/pack.bat`)
2. Playnite → Add-ons → Install from file
3. Token em https://wishlist-hub.paz.poa.br/ferramentas#playnite

**Nota:** o Hub já aceita PascalCase desde o fix server-side — plugins 1.1.0 também voltam a autenticar após o deploy do Hub. O 1.1.1 alinha o cliente à API.

### Addon browser
Manifestos em `ci/`. Ver `ci/ADDON_BROWSER.md`.
