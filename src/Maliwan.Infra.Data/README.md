## Banco de dados

Abra o "Package Manager Console" e selecione o projeto "DigaX.Infra.Data" como opção do "Default project"

Comando para gerar uma nova migration
```bash
Add-Migration NomeDaMigration -Context DigaXDbContext -OutputDir Contexts/DigaXDb/Migrations
```
Para aplicar a nova migration no banco de dados.
```bash
Update-Database -Context DigaXDbContext
```

Para desfazer a ultima migration.
```bash
Update-Database NomeDaPenultimaMigration -Context DigaXDbContext
Remove-Migration -Context DigaXDbContext
```

Documentação das [migrations.](https://docs.microsoft.com/pt-br/ef/core/managing-schemas/migrations/)
