## Banco de dados

### Maliwan Context

Documentação das [migrations.](https://docs.microsoft.com/pt-br/ef/core/managing-schemas/migrations/)

Abra o "Package Manager Console" e selecione o projeto "Maliwan.Infra.Data" como opção do "Default project"

Comando para gerar uma nova migration
```bash
Add-Migration NomeDaMigration -Context MaliwanDbContext -OutputDir Contexts/MaliwanDb/Migrations
```
Para aplicar a nova migration no banco de dados.
```bash
Update-Database -Context MaliwanDbContext
```

Para desfazer a ultima migration.
```bash
Update-Database NomeDaPenultimaMigration -Context MaliwanDbContext
Remove-Migration -Context MaliwanDbContext
```

### Identity Context

Comando para gerar uma nova migration
```bash
Add-Migration NomeDaMigration -Context IdentityDbContext -OutputDir Contexts/IdentityDb/Migrations
```
Para aplicar a nova migration no banco de dados.
```bash
Update-Database -Context IdentityDbContext
```

Para desfazer a ultima migration.
```bash
Update-Database NomeDaPenultimaMigration -Context IdentityDbContext
Remove-Migration -Context IdentityDbContext
```