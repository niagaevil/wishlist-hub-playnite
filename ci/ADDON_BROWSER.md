# Submissão ao Playnite Add-on Browser

Fonte oficial: [JosefNemec/PlayniteAddonDatabase](https://github.com/JosefNemec/PlayniteAddonDatabase)

## Pré-requisitos

1. Release GitHub `v1.1.1` com o arquivo `WishlistHub_Playnite_1_1_1.pext`
2. `ci/installer_manifest.yaml` apontando para o `PackageUrl` da release (já preparado)
3. Validar com Toolbox: `Toolbox.exe verify installer ci\installer_manifest.yaml` e `Toolbox.exe verify addon ci\addon_browser_manifest.yaml`

## Passos

1. Fork de `JosefNemec/PlayniteAddonDatabase`
2. Adicionar o arquivo:
   `addons/generic/WishlistHub_Playnite.yaml`
   (conteúdo = cópia de `ci/addon_browser_manifest.yaml` deste repo)
3. Abrir Pull Request
4. Após o merge, o add-on aparece em:
   - Playnite → Add-ons → Browse → Generic → **Wishlist Hub**
   - Web: https://playnite.link/addons.html#WishlistHub_Playnite
   - URI de instalação: `playnite://playnite/installaddon/WishlistHub_Playnite`

## Enquanto o PR não for mergeado

Use sideload: Add-ons → Install from file → `.pext` da Release.
