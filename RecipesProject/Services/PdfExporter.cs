using System;
using System.IO;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RecipesProject.Models;

namespace RecipesProject.Services
{
    public class PdfExporter
    {
        public PdfExporter()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public void Export(Recipe recipe, string filePath)
        {
            QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                    page.Header().Element(c => ComposeHeader(c, recipe));
                    page.Content().Element(c => ComposeContent(c, recipe));
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Страница ");
                        text.CurrentPageNumber();
                    });
                });
            }).GeneratePdf(filePath);
        }

        private void ComposeHeader(QuestPDF.Infrastructure.IContainer container, Recipe recipe)
        {
            container.Column(column =>
            {
                column.Item().Text(recipe.Title ?? "Без названия")
                       .FontSize(24).Bold().FontColor(Colors.Red.Darken2);
                column.Item().PaddingVertical(3);

                if (!string.IsNullOrEmpty(recipe.Description))
                {
                    column.Item().Text(recipe.Description)
                           .FontSize(12).Italic().FontColor(Colors.Grey.Darken1);
                }
                column.Item().PaddingVertical(3);

                column.Item().Row(row =>
                {
                    row.ConstantItem(120).Text($"⏱️ {recipe.CookingTime} мин").FontSize(11);
                    if (recipe.Servings.HasValue)
                        row.ConstantItem(100).Text($"👥 {recipe.Servings} порц.").FontSize(11);
                    if (recipe.Difficulty.HasValue)
                        row.ConstantItem(120).Text($"📊 Сложность: {recipe.Difficulty}/5").FontSize(11);
                    if (recipe.IsFavorite == 1)
                        row.ConstantItem(80).Text("⭐ Избранное").FontSize(11);
                });

                if (!string.IsNullOrEmpty(recipe.MainPhotoPath) && File.Exists(recipe.MainPhotoPath))
                    column.Item().PaddingVertical(5).MaxHeight(200).Image(recipe.MainPhotoPath);

                column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            });
        }

        private void ComposeContent(QuestPDF.Infrastructure.IContainer container, Recipe recipe)
        {
            container.Column(column =>
            {
                if (recipe.Ingredients != null && recipe.Ingredients.Count > 0)
                {
                    column.Item().Text("Ингредиенты:").FontSize(18).Bold().FontColor(Colors.Red.Darken2);
                    column.Item().PaddingVertical(5);
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        column.Item().PaddingVertical(2).Row(row =>
                        {
                            row.ConstantItem(15).Text("•").FontSize(14);
                            row.RelativeItem().Text(ingredient.Text ?? "").FontSize(13);
                        });
                    }
                    column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                }

                if (recipe.Steps != null && recipe.Steps.Count > 0)
                {
                    column.Item().Text("Приготовление:").FontSize(18).Bold().FontColor(Colors.Red.Darken2);
                    column.Item().PaddingVertical(5);
                    foreach (var step in recipe.Steps.OrderBy(s => s.StepNumber))
                    {
                        column.Item().PaddingVertical(3).Row(row =>
                        {
                            row.ConstantItem(30).Text($"{step.StepNumber}.")
                               .FontSize(14).Bold().FontColor(Colors.Red.Medium);
                            row.RelativeItem().Text(step.Description ?? "").FontSize(13);
                        });
                        if (!string.IsNullOrEmpty(step.PhotoPath) && File.Exists(step.PhotoPath))
                            column.Item().PaddingVertical(5).MaxHeight(150).Image(step.PhotoPath);
                    }
                }
            });
        }
    }
}