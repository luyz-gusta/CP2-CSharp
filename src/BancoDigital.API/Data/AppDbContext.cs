using BancoDigital.API.Domain.Entities;
using BancoDigital.API.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Agencia> Agencias => Set<Agencia>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<PessoaFisica> PessoasFisicas => Set<PessoaFisica>();
    public DbSet<PessoaJuridica> PessoasJuridicas => Set<PessoaJuridica>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Emprestimo> Emprestimos => Set<Emprestimo>();
    public DbSet<MaquinaDeCartao> MaquinasDeCartao => Set<MaquinaDeCartao>();
    public DbSet<ReceberSalario> ReceberSalarios => Set<ReceberSalario>();
    public DbSet<Contratacao> Contratacoes => Set<Contratacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agencia>(b =>
        {
            b.ToTable("AGENCIAS");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedOnAdd();
            b.Property(x => x.Nome).IsRequired().HasMaxLength(120);
            b.Property(x => x.Codigo).IsRequired().HasMaxLength(10);
            b.Property(x => x.Endereco).HasMaxLength(200);
            b.HasIndex(x => x.Codigo).IsUnique();
        });

        modelBuilder.Entity<Cliente>(b =>
        {
            b.ToTable("CLIENTES");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedOnAdd();
            b.Property(x => x.Nome).IsRequired().HasMaxLength(150);
            b.Property(x => x.Email).IsRequired().HasMaxLength(150);
            b.Property(x => x.Telefone).HasMaxLength(20);
            b.Property(x => x.DataCadastro).IsRequired();

            b.HasDiscriminator<TipoCliente>("TIPO_CLIENTE")
                .HasValue<PessoaFisica>(TipoCliente.PF)
                .HasValue<PessoaJuridica>(TipoCliente.PJ);

            b.HasOne(x => x.Agencia)
                .WithMany(a => a.Clientes)
                .HasForeignKey(x => x.AgenciaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PessoaFisica>(b =>
        {
            b.Property(x => x.Cpf).HasMaxLength(14);
            b.Property(x => x.RendaMensal).HasColumnType("NUMBER(18,2)");
            b.HasIndex(x => x.Cpf).IsUnique();
        });

        modelBuilder.Entity<PessoaJuridica>(b =>
        {
            b.Property(x => x.Cnpj).HasMaxLength(18);
            b.Property(x => x.RazaoSocial).HasMaxLength(200);
            b.Property(x => x.FaturamentoMensal).HasColumnType("NUMBER(18,2)");
            b.HasIndex(x => x.Cnpj).IsUnique();
        });

        modelBuilder.Entity<Produto>(b =>
        {
            b.ToTable("PRODUTOS");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedOnAdd();
            b.Property(x => x.Nome).IsRequired().HasMaxLength(120);
            b.Property(x => x.Descricao).HasMaxLength(500);
            b.Property(x => x.Ativo)
                .HasColumnType("NUMBER(1)")
                .HasConversion(v => v ? 1 : 0, v => v == 1);

            b.HasDiscriminator<string>("TIPO_PRODUTO")
                .HasValue<Emprestimo>("EMPRESTIMO")
                .HasValue<MaquinaDeCartao>("MAQUINA_CARTAO")
                .HasValue<ReceberSalario>("RECEBER_SALARIO");
        });

        modelBuilder.Entity<Emprestimo>(b =>
        {
            b.Property(x => x.ValorMinimo).HasColumnType("NUMBER(18,2)");
            b.Property(x => x.ValorMaximo).HasColumnType("NUMBER(18,2)");
            b.Property(x => x.TaxaJurosBaseAoMes).HasColumnType("NUMBER(9,4)");
        });

        modelBuilder.Entity<MaquinaDeCartao>(b =>
        {
            b.Property(x => x.Modelo).HasMaxLength(80);
            b.Property(x => x.TaxaMdrDebito).HasColumnType("NUMBER(9,4)");
            b.Property(x => x.TaxaMdrCredito).HasColumnType("NUMBER(9,4)");
            b.Property(x => x.AluguelMensal).HasColumnType("NUMBER(18,2)");
        });

        modelBuilder.Entity<ReceberSalario>(b =>
        {
            b.Property(x => x.EmpresaConveniada).HasMaxLength(200);
            b.Property(x => x.CnpjEmpregador).HasMaxLength(18);
        });

        modelBuilder.Entity<Contratacao>(b =>
        {
            b.ToTable("CONTRATACOES");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedOnAdd();
            b.Property(x => x.ValorSolicitado).HasColumnType("NUMBER(18,2)");
            b.Property(x => x.TaxaJurosAplicada).HasColumnType("NUMBER(9,4)");
            b.Property(x => x.ValorAprovado).HasColumnType("NUMBER(18,2)");
            b.Property(x => x.MotivoStatus).HasMaxLength(500);
            b.Property(x => x.Status).HasConversion<int>();

            b.HasOne(x => x.Cliente)
                .WithMany(c => c.Contratacoes)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Produto)
                .WithMany()
                .HasForeignKey(x => x.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
