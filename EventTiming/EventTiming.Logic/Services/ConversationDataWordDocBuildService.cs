//using Croc.CFB.Common;
//using Croc.CFB.Domain.Constants;
//using Croc.CFB.Logic.Queries;
//using Croc.CFB.Logic.Queries.AppraisalConversationData.AdditionalInfo;
//using Croc.CFB.Logic.Queries.AppraisalConversationData.Feedback;
//using Croc.CFB.Logic.Queries.AppraisalConversationData.Goals;
//using Croc.CFB.Logic.Queries.AppraisalConversationData.OtherAppraisals;
//using Croc.CFB.Logic.Queries.AppraisalConversationData.PreviousResults;
//using Croc.CFB.Logic.Queries.AppraisalConversationData.RatingLevels;
//using Croc.CFB.Logic.Queries.Appraisals.AppraisalViewEditData;
//using Croc.CFB.Logic.Queries.AppraisalView.AppraisalData;
//using Croc.CFB.Queries;
//using DocumentFormat.OpenXml;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Wordprocessing;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using static Croc.CFB.Logic.Queries.AppraisalConversationData.RatingLevels.CompetenceRatingLevelsQueryResult;

//namespace Croc.CFB.Logic.Services
//{
//    public class ConversationDataWordDocBuildService : IConversationDataWordDocBuildService
//    {
//        public class Constants
//        {
//            public const string SectionHeaderStyle = "SectionHeaderStyle";
//        }



//        private readonly IQueryHandler<CompetenceRatingLevelsQuery, CompetenceRatingLevelsQueryResult> _competenceRatingLevelsQuery;
//        private readonly IQueryHandler<FeedbackQuery, FeedbackQueryResult> _feedbackQuery;
//        private readonly IQueryHandler<AppraisalDataQuery, AppraisalDataQueryResult> _appraisalDataQuery;
//        private readonly IQueryHandler<PreviousResultsQuery, PreviousResultsQueryResult> _previousResultsQuery;
//        private readonly IQueryHandler<OtherAppraisalsQuery, OtherAppraisalsQueryResult> _otherAppraisalsQuery;
//        private readonly IQueryHandler<GoalsQuery, GoalsQueryResult> _goalsQuery;
//        private readonly IQueryHandler<AdditionalInfoQuery, AdditionalInfoQueryResult> _appraisalAdditionalInfoQuery;

//        private WordprocessingDocument CurrentDoc { get; set; }

//        public ConversationDataWordDocBuildService(IQueryHandler<CompetenceRatingLevelsQuery, CompetenceRatingLevelsQueryResult> competenceRatingLevelsQuery,
//            IQueryHandler<FeedbackQuery, FeedbackQueryResult> feedbackQuery,
//            IQueryHandler<AppraisalDataQuery, AppraisalDataQueryResult> appraisalDataQuery,
//            IQueryHandler<PreviousResultsQuery, PreviousResultsQueryResult> previousResultsQuery,
//            IQueryHandler<OtherAppraisalsQuery, OtherAppraisalsQueryResult> otherAppraisalsQuery,
//             IQueryHandler<GoalsQuery, GoalsQueryResult> goalsQuery,
//             IQueryHandler<AdditionalInfoQuery, AdditionalInfoQueryResult> appraisalAdditionalInfoQuery)
//        {
//            _competenceRatingLevelsQuery = competenceRatingLevelsQuery;
//            _feedbackQuery = feedbackQuery;
//            _appraisalDataQuery = appraisalDataQuery;
//            _previousResultsQuery = previousResultsQuery;
//            _otherAppraisalsQuery = otherAppraisalsQuery;
//            _goalsQuery = goalsQuery;
//            _appraisalAdditionalInfoQuery = appraisalAdditionalInfoQuery;
//        }

//        public void CreateAndFillDocument(WordprocessingDocument wordDoc, Guid appraisalId)
//        {
//            CurrentDoc = wordDoc;

//            wordDoc.AddMainDocumentPart();
//            Document doc = new Document();
//            wordDoc.MainDocumentPart.Document = doc;

//            StyleDefinitionsPart part = doc.MainDocumentPart.StyleDefinitionsPart;

//            if (part == null)
//            {
//                part = AddStylesPartToPackage(wordDoc);
//            }

//            RedefineNormalStyle(part);

//            CreateAndAddHeaderParagraphStyle(part,
//                Constants.SectionHeaderStyle,
//                string.Empty,
//                string.Empty);


//            Body body = new Body();

//            CreateAndFillHeader(appraisalId, body);

//            //таблицы с оценками
//            CreateAndFillCompetencesRatingLevelsSection(appraisalId, body);

//            //отзывы
//            CreateAndFillFeedbackSection(appraisalId, body);

//            //результаты и рекомендации
//            CreateAndFillResultAndRecommendationsSection(appraisalId, body);

//            //другие анкетирования
//            CreateAndFillOtherAppraisalsSection(appraisalId, body);

//            //предыдущие цели и договоренности
//            CreateAndFillGoalsSection(appraisalId, body);

//            //доп блоки
//            CreateAndFillAdditionalInfoSection(appraisalId, body);

//            doc.Append(body);

//        }

//        private void RedefineNormalStyle(StyleDefinitionsPart styleDefinitionsPart)
//        {
//            // Access the root element of the styles part.
//            Styles styles = styleDefinitionsPart.Styles;
//            if (styles == null)
//            {
//                styleDefinitionsPart.Styles = new Styles();
//                styleDefinitionsPart.Styles.Save();
//            }

//            Style normalStyle = new Style();
//            normalStyle.StyleId = "Normal";
//            normalStyle.Append(new Name() { Val = "Normal" });
//            normalStyle.Append(new BasedOn() { Val = "Normal" });

//            RunProperties normalProperties = new RunProperties();

//            RunFonts defaultFont = new RunFonts();
//            defaultFont.Ascii = "Calibri Light";
//            normalProperties.Append(defaultFont);

//            normalProperties.Append(new FontSize() { Val = "24" });
//            normalStyle.Append(normalProperties);

//            styles.Append(normalStyle);
//        }

//        private void CreateAndFillHeader(Guid appraisalId, Body body)
//        {

//            var appraisalData = _appraisalDataQuery.Execute(new AppraisalDataQuery
//            {
//                AppraisalId = appraisalId
//            });

//            if (appraisalData == null)
//                return;

//            Text appraiseeName = new Text() { Text = appraisalData.AppraiseeUserDisplayName };
//            Text appraisalDates = new Text() { Text = $"{appraisalData.AppraisalTypeName} {appraisalData.AppraisalPeriodStart.Value.ToString("dd.MM.yyyy")} – {appraisalData.AppraisalPeriodEnd.Value.ToString("dd.MM.yyyy")}" };
//            Text appraiseePhoneNumberText = !string.IsNullOrEmpty(appraisalData.AppraiseeUserPhoneNumber) ?
//                new Text { Text = $"Телефон: {appraisalData.AppraiseeUserPhoneNumber}" } : null;


//            Paragraph headerParagraph = new Paragraph();

//            ParagraphProperties headerParagraphProperties = new ParagraphProperties();
//            headerParagraphProperties.Append(new ParagraphStyleId() { Val = "Normal" });
//            headerParagraphProperties.Append(new Justification() { Val = JustificationValues.Center });
//            headerParagraphProperties.Append(new ParagraphMarkRunProperties());
//            //headerParagraphProperties.Append(new FontSize() { Val = "22" });

//            Run headerNameRun = new Run();
//            RunProperties headerNameRunProperties = new RunProperties();
//            headerNameRunProperties.Append(new FontSize() { Val = "44" });


//            headerNameRun.Append(headerNameRunProperties);
//            headerNameRun.Append(appraiseeName);
//            headerNameRun.Append(new Break());

//            Run headerTypeAndDatesRun = new Run();
//            RunProperties headerTypeAndDatesRunProperties = new RunProperties();
//            headerTypeAndDatesRunProperties.Append(new FontSize() { Val = "28" });

//            headerTypeAndDatesRun.Append(headerTypeAndDatesRunProperties);
//            headerTypeAndDatesRun.Append(appraisalDates);
//            headerTypeAndDatesRun.Append(new Break());

//            headerParagraph.Append(headerParagraphProperties);
//            headerParagraph.Append(headerNameRun);

//            if(appraiseePhoneNumberText != null)
//            {
//                Run headerAppraiseePhoneNumberRun = new Run();
//                RunProperties headerAppraiseePhoneNumberRunProperties = new RunProperties();
//                headerAppraiseePhoneNumberRunProperties.Append(new FontSize() { Val = "28" });

//                headerAppraiseePhoneNumberRun.Append(headerAppraiseePhoneNumberRunProperties);
//                headerAppraiseePhoneNumberRun.Append(appraiseePhoneNumberText);
//                headerAppraiseePhoneNumberRun.Append(new Break());

//                headerParagraph.Append(headerAppraiseePhoneNumberRun);
//            }

//            headerParagraph.Append(headerTypeAndDatesRun);

//            body.Append(headerParagraph);
//        }

//        private void CreateAndFillCompetencesRatingLevelsSection(Guid appraisalId, Body body)
//        {
//            var result = _competenceRatingLevelsQuery.Execute(new CompetenceRatingLevelsQuery
//            {
//                AppraisalId = appraisalId
//            });

//            if (result == null || result.CompetenceGroups == null || !result.CompetenceGroups.Any())
//                return;

//            foreach (var competenceGroup in result.CompetenceGroups)
//            {
//                var competenceMetrics = result.RatingLevelMetrics.Where(m => m.CompetenceId == competenceGroup.CompetenceId).ToList();

//                body.Append(GetSectionTableNameParagraph(competenceGroup.CompetenceName));
//                if (competenceGroup.CompetenceVisualOptionCode == VisualOptionConstants.ScaleVisualOptionCode ||
//                    competenceGroup.CompetenceVisualOptionCode == VisualOptionConstants.GradientScaleVisialOptionCode)
//                {
//                    body.Append(GetSelectedRatingLevelsTableWithAverage(competenceGroup.SelectedRatingLevels.ToList(), competenceMetrics));
//                    body.Append(GetBreakParagraph());
//                }
//                else if (competenceGroup.CompetenceVisualOptionCode == VisualOptionConstants.CheckBoxVisualOptionCode ||
//                        competenceGroup.CompetenceVisualOptionCode == VisualOptionConstants.DropDownListVisualOptionCode ||
//                        competenceGroup.CompetenceVisualOptionCode == VisualOptionConstants.RadioButtonVisualOptionCode)
//                {
//                    body.Append(GetSelectedRatingLevelsTable(competenceGroup.SelectedRatingLevels.ToList(), competenceMetrics));
//                    body.Append(GetBreakParagraph());
//                }
//            }
//        }

//        private void CreateAndFillAdditionalInfoSection(Guid appraisalId, Body body)
//        {
//            var additionalInfo = _appraisalAdditionalInfoQuery.Execute(new AdditionalInfoQuery { AppraisalId = appraisalId });

//            if (additionalInfo == null || additionalInfo.InfoBlocks == null || additionalInfo.InfoBlocks.Block == null)
//                return;

//            body.Append(GetSectionTitleParagraph($"Информация из внешних систем актуальна на {additionalInfo.InfoBlocks.creation_date}:"));

//            //пройдемся по всем блокам
//            foreach (var block in additionalInfo.InfoBlocks.Block)
//            {
//                body.Append(GetBreakParagraph());
//                body.Append(GetSectionTitleParagraph(block.title));

//                if (block.Table != null)
//                {
//                    bool isFirstTable = true;
//                    foreach (var blockTable in block.Table)
//                    {
//                        //для первой таблицы отступ не добавляем
//                        if (!isFirstTable)
//                            body.Append(GetBreakParagraph());
//                        else
//                            isFirstTable = false;

//                        body.Append(GetAdditionalInfoTitleParagraph(blockTable.title));
//                        body.Append(GetAdditionalInfoTable(blockTable));
//                    }
//                }

//                if (block.Paragraph != null)
//                {
//                    foreach(var blockParagraph in block.Paragraph)
//                    {
//                        body.Append(GetBreakParagraph());
//                        body.Append(GetAdditionalInfoTextParagraph(blockParagraph.Value));
//                    }
//                }
//            }
//        }

//        private void CreateAndFillGoalsSection(Guid appraisalId, Body body)
//        {
//            var goals = _goalsQuery.ExecuteAsync(new GoalsQuery
//            {
//                CurrentAppraisalId = appraisalId
//            }).Result.Goals;

//            if (goals == null || !goals.Any())
//                return;

//            body.Append(GetSectionTitleParagraph("Предыдущие цели и договоренности"));
//            body.Append(GetGoalsTable(goals));
//            body.Append(GetBreakParagraph());

//        }

//        private void CreateAndFillOtherAppraisalsSection(Guid appraisalId, Body body)
//        {
//            var otherAppraisalsDataItems = _otherAppraisalsQuery.Execute(new OtherAppraisalsQuery
//            {
//                CurrentAppraisalId = appraisalId
//            }).AppraisalsData;

//            if (otherAppraisalsDataItems == null || !otherAppraisalsDataItems.Any())
//                return;

//            foreach (var otherAppraisalDataItem in otherAppraisalsDataItems)
//            {
//                //сначала пишем название анкетирования
//                body.Append(GetOtherAppraisalTypeNameParagraph(otherAppraisalDataItem.AppraisalType));

//                //далее формируем и добавляем таблицы с оценками
//                foreach (var competenceDataItem in otherAppraisalDataItem.CompetenceDataItems)
//                {
//                    body.Append(GetSectionTableNameParagraph(competenceDataItem.CompetenceName));
//                    body.Append(GetOtherAppraisalRatingLevelsTable(competenceDataItem.SelectedRatingLevels.ToList())); ;
//                    body.Append(GetBreakParagraph());
//                }

//                //далее формируем секцию отзывов
//                foreach (var feedbackItem in otherAppraisalDataItem.FeedbackItems)
//                {
//                    var nameTitle = feedbackItem.AppraisalParticipantDateCompleted.HasValue ? $"{feedbackItem.AppraisalParticipantDisplayName} {feedbackItem.AppraisalParticipantDateCompleted.Value.ToString("dd.MM.yyyy")}"
//                        : feedbackItem.AppraisalParticipantDisplayName;

//                    //параграф с именем
//                    body.Append(GetFeedbackSectionParticipantNameParagraph(nameTitle, false));
//                    //под ним параграф с отзывом
//                    body.Append(GetFeedbackSectionParticipantFeedbackParagraph(feedbackItem.AppraisalParticipantTextFeedback, false));
//                    body.Append(GetBreakParagraph());

//                }

//            }
//        }

//        private void CreateAndFillResultAndRecommendationsSection(Guid appraisalId, Body body)
//        {
//            var appraisalResultsRecommendations = _previousResultsQuery.Execute(new PreviousResultsQuery
//            {
//                CurrentAppraisalId = appraisalId
//            }).AppraisalResultsRecommendations;

//            if (!string.IsNullOrEmpty(appraisalResultsRecommendations))
//            {
//                appraisalResultsRecommendations = RemoveHtmlTags(appraisalResultsRecommendations);
//                body.Append(GetSectionTitleParagraph("Предыдущие результаты и рекомендации"));
//                body.Append(GetResultsAndRecommendationsParagraph(appraisalResultsRecommendations));
//            }
//        }

//        private Paragraph GetResultsAndRecommendationsParagraph(string appraisalResultsRecommendations)
//        {
//            Paragraph paragraph = new Paragraph();

//            ParagraphProperties paragraphProperties = new ParagraphProperties();

//            Run run = new Run();
//            RunProperties runProperties = new RunProperties();

//            FontSize headerFontSize = new FontSize();
//            headerFontSize.Val = new DocumentFormat.OpenXml.StringValue("22");

//            runProperties.FontSize = headerFontSize;

//            run.Append(runProperties);

//            //обрабатываем символ переноса на новую строку
//            ProcessTextWithNewLinesAndLinks(appraisalResultsRecommendations, paragraph);

//            paragraph.Append(paragraphProperties);
//            //paragraph.Append(run);

//            return paragraph;
//        }

//        private void ProcessTextWithNewLinesAndLinks(string text,  Paragraph paragraph)
//        {
//            //чтобы учесть обычные переносы, преобразуем их сначала в html line breaks
//            text = HtmlHelper.ConvertNewLinesToHtmlLineBreaks(text);

//            if (text.IndexOf(HtmlHelper.LineBreakTag, StringComparison.OrdinalIgnoreCase) >= 0)
//            {
//                var textItems = text.Split(new[] { HtmlHelper.LineBreakTag }, StringSplitOptions.RemoveEmptyEntries);

//                for (int i = 0; i < textItems.Length; i++)
//                {
//                    if (textItems[i].IndexOf(HtmlHelper.AnchorClose, StringComparison.OrdinalIgnoreCase) >= 0)
//                        ProcessLinks(textItems[i], paragraph);
//                    else
//                    {
//                        Text txt = new Text() { Text = textItems[i] };

//                        var run = GetSimpleRunElement();
//                        run.Append(txt);
//                        paragraph.Append(run);
//                    }

//                    if (i != textItems.Length - 1)
//                    {
//                        var run = GetSimpleRunElement();
//                        run.Append(new Break());
//                        paragraph.Append(run);
//                    }
//                }
//            }
//            else
//            {
//                if (text.IndexOf(HtmlHelper.AnchorClose, StringComparison.OrdinalIgnoreCase) >= 0)
//                    ProcessLinks(text, paragraph);
//                else
//                {
//                    Text titleText = new Text() { Text = text };
//                    var run = GetSimpleRunElement();
//                    run.Append(titleText);
//                    paragraph.Append(run);
//                }

//            }
//        }

//        private Run GetSimpleRunElement()
//        {
//            Run run = new Run();
//            RunProperties runProperties = new RunProperties();

//            FontSize headerFontSize = new FontSize();
//            headerFontSize.Val = new DocumentFormat.OpenXml.StringValue("22");

//            runProperties.FontSize = headerFontSize;

//            run.Append(runProperties);

//            return run;
//        }

//        private void ProcessLinks(string text, Paragraph paragraph)
//        {
//            //разобьем строку по окончанию тэга </a>
//            var textItems = text.Split( new[] {HtmlHelper.AnchorClose }, StringSplitOptions.RemoveEmptyEntries);

//            for (int i = 0; i < textItems.Length; i++)
//            {
//                //в одном элементе содержится ссылка целиком
//                //найдем индекс
//                var startTagIndex = textItems[i].IndexOf(HtmlHelper.AnchorOpen, StringComparison.OrdinalIgnoreCase);

//                //если в подстроке нет тэга, просто добавим текст
//                if (startTagIndex == -1)
//                {
//                    var txt = GetTextElementWithPreservedSpace(textItems[i]);
//                    var run = GetSimpleRunElement();
//                    run.Append(txt);
//                    paragraph.Append(run);
//                }
//                else
//                {
//                    //в противном случае выделим сначала часть с текстом
//                    var textPart = textItems[i].Substring(0, (startTagIndex));
//                    var textPartText = GetTextElementWithPreservedSpace(textPart);
//                    var run = GetSimpleRunElement();
//                    run.Append(textPartText);
//                    paragraph.Append(run);

//                    //после этого выделим ссылку
//                    //добавим к значениею окончание ссылки - удаляется при сплите

//                    var htmlTag = textItems[i].Substring(startTagIndex);

//                    AddLinkToParagraph(paragraph, (htmlTag + HtmlHelper.AnchorClose));
//                }
//            }
//        }

//        private Text GetTextElementWithPreservedSpace(string text)
//        {
//            return new Text() { Text = text, Space = SpaceProcessingModeValues.Preserve };
//        }

//        private void AddLinkToParagraph(Paragraph paragraph, string text)
//        {

//            const string textPartStart = ">";
//            const string textPartFinish = "</";

//            const string hrefRegexPattern = "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";

//            //выделим часть с текстом  
//            string textValuePart = text.Substring(text.IndexOf(textPartStart) + textPartStart.Length, text.IndexOf(textPartFinish) - text.IndexOf(textPartStart) - textPartStart.Length);
            
//            //выделим ссылку из href
//            var match = Regex.Match(text, hrefRegexPattern);

//            if (!match.Success)
//                throw new Exception("В ссылке отсутствует атрибут href");

//            string hrefValuePart = match.Groups[1].Value;

//            var rel = CurrentDoc.MainDocumentPart.AddHyperlinkRelationship(new Uri(hrefValuePart, UriKind.RelativeOrAbsolute), true);

//            paragraph.Append(
//                new Hyperlink(
//                    new Run(
//                        new RunProperties(
//                            new RunStyle { Val = "Hyperlink", },
//                            new Underline { Val = UnderlineValues.Single },
//                            new Color { ThemeColor = ThemeColorValues.Hyperlink }),
//                        new Text { Text = textValuePart }
//                    ))
//                { History = OnOffValue.FromBoolean(true), Id = rel.Id }

//                );
//        }

//        /// <summary>
//        /// Метод удаления html-разметки - для данных, которые мигрированы из HRMS
//        /// </summary>
//        /// <param name="appraisalResultsRecommendations"></param>
//        /// <returns></returns>
//        private string RemoveHtmlTags(string appraisalResultsRecommendations)
//        {
//            StringBuilder sb = new StringBuilder(appraisalResultsRecommendations);

//            sb
//                .Replace("<p>", HtmlHelper.LineBreakTag)
//                .Replace("<P>", HtmlHelper.LineBreakTag)
//                .Replace("<strong>", string.Empty)
//                .Replace("<STRONG>", string.Empty)
//                .Replace("</strong>", string.Empty)
//                .Replace("</STRONG>", string.Empty)
//                .Replace("<em>", string.Empty)
//                .Replace("<EM>", string.Empty)
//                .Replace("</em>", string.Empty)
//                .Replace("</EM>", string.Empty)
//                .Replace("</p>", HtmlHelper.LineBreakTag)
//                .Replace("</P>", HtmlHelper.LineBreakTag)
//                //последний элемент в списке
//                //.Replace("</li></ul>", HtmlHelper.LineBreakTag)
//                //.Replace("</LI></UL>", HtmlHelper.LineBreakTag)
//                .Replace("<ul>", string.Empty)
//                .Replace("<UL>", string.Empty)
//                .Replace("<li>", HtmlHelper.LineBreakTag)
//                .Replace("<LI>", HtmlHelper.LineBreakTag)
//                .Replace("</ul>", string.Empty)
//                .Replace("</UL>", string.Empty)
//                .Replace("</li>", HtmlHelper.LineBreakTag)
//                .Replace("</LI>", HtmlHelper.LineBreakTag)
//                .Replace("&quot;", "\"")
//                .Replace("&QUOT;", "\"")
//                .Replace("&amp;", "&")
//                .Replace("&AMP;", "&")
//                .Replace("&nbsp;", " ")
//                .Replace("&NBSP;", " ")
//                .Replace("<blockquote>", "")
//                .Replace("<BLOCKQUOTE>", "")
//                .Replace("</blockquote>", "")
//                .Replace("</BLOCKQUOTE>", "")
//                .Replace("<br>", HtmlHelper.LineBreakTag)
//                .Replace("<BR>", HtmlHelper.LineBreakTag)
//                .Replace("</BLOCKQUOTE><P>", HtmlHelper.LineBreakTag)
//                .Replace("</blockquote><p>", HtmlHelper.LineBreakTag)
//                .Replace("<A ", "<a ")
//                .Replace("</A>", "</a>")
//                .Replace("&laquo;", "<<")
//                .Replace("&raquo;", ">>")
//                .Replace("&ndash;", "–")
//                .Replace("&mdash", "–")
//                ;

//            return sb.ToString();
//        }

//        private void CreateAndFillFeedbackSection(Guid appraisalId, Body body)
//        {
//            body.Append(GetSectionTitleParagraph("Отзывы"));
//            var feedBackHintBlocks = _feedbackQuery.Execute(new FeedbackQuery { AppraisalId = appraisalId }).HintBlocks;

//            if (feedBackHintBlocks == null || !feedBackHintBlocks.Any())
//                return;

//            //сначала берем блок, где нет подсказки, потом все остальные
//            var emptyHintBlock = feedBackHintBlocks.FirstOrDefault(b => string.IsNullOrEmpty(b.TextHint));

//            if (emptyHintBlock != null)
//            {

//                foreach (var emptyHintItem in emptyHintBlock.HintBlockFeedbackItems)
//                {
//                    //параграф с именем
//                    body.Append(GetFeedbackSectionParticipantNameParagraph(emptyHintItem.ParticipantDisplayName, false));
//                    //под ним параграф с отзывом
//                    body.Append(GetFeedbackSectionParticipantFeedbackParagraph(emptyHintItem.FeedbackText, false));
//                    body.Append(GetBreakParagraph());
//                }

//            }

//            //обрабатываем блоки с заполненным отзывом
//            var filledHintBlocks = feedBackHintBlocks.Where(b => !string.IsNullOrEmpty(b.TextHint)).ToList();

//            foreach (var filledHintBlock in filledHintBlocks)
//            {
//                //сначала добавляем подсказку
//                body.Append(GetFeedbackSectionHintValueParagraph(filledHintBlock.TextHint));
//                //body.Append(GetBreakParagraph());
//                //поотом проходимся и добавляем отзывы
//                foreach (var filledHintItem in filledHintBlock.HintBlockFeedbackItems)
//                {
//                    //параграф с именем
//                    body.Append(GetFeedbackSectionParticipantNameParagraph(filledHintItem.ParticipantDisplayName, true));
//                    //под ним параграф с отзывом
//                    body.Append(GetFeedbackSectionParticipantFeedbackParagraph(filledHintItem.FeedbackText, true));
//                    body.Append(GetBreakParagraph());
//                }
//            }
//        }

//        private Paragraph GetBreakParagraph()
//        {
//            Paragraph breakParagraph = new Paragraph();

//            Run paragrapghRun = new Run();
//            //paragrapghRun.AppendChild(new Break());
//            //paragrapghRun.AppendChild(new Break());

//            breakParagraph.Append(paragrapghRun);

//            return breakParagraph;
//        }

//        private Table GetSelectedRatingLevelsTableWithAverage(List<SelectedRatingLevel> selectedLevels, List<ParticipantRatingLevelMetrics> metrics)
//        {

//            Table table = new Table();

//            TableProperties props = GetBaseTableProperties();
//            table.AppendChild<TableProperties>(props);

//            var headerRow = new TableRow();

//            var headerCell1 = new TableCell();
//            FormHeaderCell(headerCell1, "Опрашиваемый");

//            var headerCell2 = new TableCell();
//            FormHeaderCell(headerCell2, "Текущее значение по сотруднику");


//            var headerCell3 = new TableCell();
//            FormHeaderCell(headerCell3, "Предыдущее значение по сотруднику");

//            var headerCell4 = new TableCell();
//            FormHeaderCell(headerCell4, "Среднее значение по всем сотрудникам");

//            headerRow.Append(headerCell1);
//            headerRow.Append(headerCell2);
//            headerRow.Append(headerCell3);
//            headerRow.Append(headerCell4);

//            table.Append(headerRow);

//            foreach (var selectedLevel in selectedLevels)
//            {
//                var tr = new TableRow();

//                var nameCell = new TableCell();
//                nameCell.Append(new Paragraph(new Run(new Text(selectedLevel.AppraisalParticipantDisplayName))));
//                nameCell.Append(new TableCellProperties(
//                    new TableCellWidth { Type = TableWidthUnitValues.Auto }));

//                tr.Append(nameCell);

//                var currentLevel = new TableCell();
//                currentLevel.Append(new Paragraph(new Run(new Text(selectedLevel.RatingLevelName))));
//                currentLevel.Append(new TableCellProperties(
//                    new TableCellWidth { Type = TableWidthUnitValues.Auto }));

//                tr.Append(currentLevel);

//                var currentParticipantMetrics = metrics.FirstOrDefault(m => m.AppraisalParticipantId == selectedLevel.AppraisalParticipantId);

//                if (currentParticipantMetrics != null)
//                {

//                    var previousLevel = new TableCell();
//                    if (!string.IsNullOrEmpty(currentParticipantMetrics.PreviousRatingLevelName))
//                    {
//                        previousLevel.Append(new Paragraph(new Run(new Text(currentParticipantMetrics.PreviousRatingLevelName))));
//                    }
//                    else
//                    {
//                        previousLevel.Append(new Paragraph(new Run(new Text("-"))));

//                    }
//                    previousLevel.Append(new TableCellProperties(
//                        new TableCellWidth { Type = TableWidthUnitValues.Auto }));

//                    tr.Append(previousLevel);


//                    var averageValue = new TableCell();

//                    if (currentParticipantMetrics.AvgRatingLevelValue.HasValue)
//                    {
//                        averageValue.Append(new Paragraph(new Run(new Text(currentParticipantMetrics.AvgRatingLevelValue.ToString()))));
//                    }
//                    else
//                    {
//                        averageValue.Append(new Paragraph(new Run(new Text("-"))));

//                    }
//                    averageValue.Append(new TableCellProperties(
//                        new TableCellWidth { Type = TableWidthUnitValues.Auto }));

//                    tr.Append(averageValue);

//                }
//                table.Append(tr);
//            }

//            return table;


//        }

//        private void FormHeaderCell(TableCell cell, string text)
//        {
//            var cellRun = new Run();
//            var cellRunProperties = new RunProperties();
//            cellRunProperties.Append(new FontSize() { Val = "28" });

//            var cellText = new Text(text);
//            cellRun.Append(cellRunProperties);
//            cellRun.Append(cellText);

//            cell.Append(new Paragraph(cellRun));
//            cell.Append(new TableCellProperties(
//                new TableCellWidth { Type = TableWidthUnitValues.Auto }));
//        }

//        private Table GetSelectedRatingLevelsTable(List<SelectedRatingLevel> selectedLevels, List<ParticipantRatingLevelMetrics> metrics)
//        {

//            Table table = new Table();

//            TableProperties props = GetBaseTableProperties();

//            table.AppendChild<TableProperties>(props);

//            var headerRow = new TableRow();

//            var headerCell1 = new TableCell();
//            FormHeaderCell(headerCell1, "Опрашиваемый");

//            var headerCell2 = new TableCell();
//            FormHeaderCell(headerCell2, "Текущее значение по сотруднику");

//            var headerCell3 = new TableCell();
//            FormHeaderCell(headerCell3, "Предыдущее значение по сотруднику");


//            headerRow.Append(headerCell1);
//            headerRow.Append(headerCell2);
//            headerRow.Append(headerCell3);

//            table.Append(headerRow);

//            foreach (var selectedLevel in selectedLevels)
//            {
//                var tr = new TableRow();

//                var nameCell = new TableCell();
//                nameCell.Append(new Paragraph(new Run(new Text(selectedLevel.AppraisalParticipantDisplayName))));
//                nameCell.Append(new TableCellProperties(
//                    new TableCellWidth { Type = TableWidthUnitValues.Auto }));

//                tr.Append(nameCell);

//                var currentLevel = new TableCell();
//                currentLevel.Append(new Paragraph(new Run(new Text(selectedLevel.RatingLevelName))));
//                currentLevel.Append(new TableCellProperties(
//                    new TableCellWidth { Type = TableWidthUnitValues.Auto }));

//                tr.Append(currentLevel);

//                var currentParticipantMetrics = metrics.FirstOrDefault(m => m.AppraisalParticipantId == selectedLevel.AppraisalParticipantId);

//                if (currentParticipantMetrics != null)
//                {

//                    var previousLevel = new TableCell();
//                    if (!string.IsNullOrEmpty(currentParticipantMetrics.PreviousRatingLevelName))
//                    {
//                        previousLevel.Append(new Paragraph(new Run(new Text(currentParticipantMetrics.PreviousRatingLevelName))));
//                    }
//                    else
//                    {
//                        previousLevel.Append(new Paragraph(new Run(new Text("-"))));

//                    }
//                    previousLevel.Append(new TableCellProperties(
//                        new TableCellWidth { Type = TableWidthUnitValues.Auto }));

//                    tr.Append(previousLevel);
//                }

//                table.Append(tr);
//            }

//            return table;


//        }

//        private Table GetOtherAppraisalRatingLevelsTable(List<OtherAppraisalsQueryResult.SelectedRatingLevel> selectedLevels)
//        {

//            Table table = new Table();

//            TableProperties props = GetBaseTableProperties();

//            table.AppendChild<TableProperties>(props);

//            var headerRow = new TableRow();

//            var headerCell1 = new TableCell();
//            FormHeaderCell(headerCell1, "Опрашиваемый");


//            var headerCell2 = new TableCell();
//            FormHeaderCell(headerCell2, "Значение по сотруднику");


//            var headerCell3 = new TableCell();
//            FormHeaderCell(headerCell3, "Дата завершения");


//            headerRow.Append(headerCell1);
//            headerRow.Append(headerCell2);
//            headerRow.Append(headerCell3);

//            table.Append(headerRow);

//            foreach (var selectedLevel in selectedLevels)
//            {
//                var tr = new TableRow();

//                var nameCell = new TableCell();
//                nameCell.Append(new Paragraph(new Run(new Text(selectedLevel.AppraisalParticipantDisplayName))));
//                nameCell.Append(new TableCellProperties(
//                    new TableCellWidth { Type = TableWidthUnitValues.Auto }));

//                tr.Append(nameCell);

//                var currentLevel = new TableCell();
//                currentLevel.Append(new Paragraph(new Run(new Text(selectedLevel.RatingLevelName))));
//                currentLevel.Append(new TableCellProperties(
//                    new TableCellWidth { Type = TableWidthUnitValues.Auto }));

//                tr.Append(currentLevel);

//                var dateCompletedCell = new TableCell();
//                if (selectedLevel.AppraisalParticipantDateCompleted.HasValue)
//                {
//                    dateCompletedCell.Append(new Paragraph(new Run(new Text(selectedLevel.AppraisalParticipantDateCompleted.Value.ToString("dd.MM.yyyy")))));
//                }
//                else
//                {
//                    dateCompletedCell.Append(new Paragraph(new Run(new Text("-"))));

//                }
//                dateCompletedCell.Append(new TableCellProperties(
//                    new TableCellWidth { Type = TableWidthUnitValues.Auto }));

//                tr.Append(dateCompletedCell);


//                table.Append(tr);
//            }

//            return table;
//        }

//        private Table GetGoalsTable(List<GoalsQueryResult.GoalItem> goals)
//        {

//            Table table = new Table();

//            TableProperties props = GetBaseTableProperties();
//            table.AppendChild<TableProperties>(props);

//            var headerRow = new TableRow();

//            var headerCell1 = new TableCell();
//            FormHeaderCell(headerCell1, "Цель");

//            var headerCell2 = new TableCell();
//            FormHeaderCell(headerCell2, "Контрольная дата");

//            var headerCell3 = new TableCell();
//            FormHeaderCell(headerCell3, "Статус");

//            var headerCell4 = new TableCell();
//            FormHeaderCell(headerCell4, "Результат");

//            headerRow.Append(headerCell1);
//            headerRow.Append(headerCell2);
//            headerRow.Append(headerCell3);
//            headerRow.Append(headerCell4);

//            table.Append(headerRow);

//            foreach (var goal in goals)
//            {
//                var tr = new TableRow();

//                var descriptionCell = new TableCell();

//                Run descriptionRun = new Run();

//                ProcessTextWithNewLines(goal.Description, descriptionRun);

//                descriptionCell.Append(new Paragraph(descriptionRun));
//                descriptionCell.Append(new TableCellProperties(
//                    new TableCellWidth { Type = TableWidthUnitValues.Auto }));
//                tr.Append(descriptionCell);


//                var dateCell = new TableCell();
//                if (goal.KeyDate.HasValue)
//                {
//                    dateCell.Append(new Paragraph(new Run(new Text(goal.KeyDate.Value.ToString("dd.MM.yyyy")))));
//                }
//                else
//                {
//                    dateCell.Append(new Paragraph(new Run(new Text("-"))));

//                }
//                dateCell.Append(new TableCellProperties(
//                    new TableCellWidth { Type = TableWidthUnitValues.Auto }));

//                tr.Append(dateCell);


//                var statusCell = new TableCell();
//                if (!string.IsNullOrEmpty(goal.StatusName))
//                {
//                    statusCell.Append(new Paragraph(new Run(new Text(goal.StatusName))));
//                }
//                else
//                {
//                    statusCell.Append(new Paragraph(new Run(new Text("-"))));

//                }
//                statusCell.Append(new TableCellProperties(
//                    new TableCellWidth { Type = TableWidthUnitValues.Auto }));

//                tr.Append(statusCell);



//                var resultCell = new TableCell();
//                if (!string.IsNullOrEmpty(goal.ResultName))
//                {
//                    resultCell.Append(new Paragraph(new Run(new Text(goal.ResultName))));
//                }
//                else
//                {
//                    resultCell.Append(new Paragraph(new Run(new Text("-"))));

//                }
//                resultCell.Append(new TableCellProperties(
//                    new TableCellWidth { Type = TableWidthUnitValues.Auto }));

//                tr.Append(resultCell);


//                table.Append(tr);
//            }

//            return table;
//        }
//        private Table GetAdditionalInfoTable(InfoBlocksBlockTable tableObject)
//        {
//            Table table = new Table();

//            TableProperties props = GetBaseTableProperties();

//            table.AppendChild<TableProperties>(props);

//            //сначала динамически сформируем строку-заголовок 
//            var headerRow = new TableRow();
//            bool anyHeaderColumnFilled = false;

//            foreach (var header in tableObject.TableHeader)
//            {
//                if (string.IsNullOrEmpty(header))
//                    continue;

//                anyHeaderColumnFilled = true;

//                var headerCell = new TableCell();
//                FormHeaderCell(headerCell, header);
//                headerRow.Append(headerCell);
//            }

//            if (anyHeaderColumnFilled)
//                table.Append(headerRow);

//            //потом динамически добавим строки
//            foreach (var row in tableObject.TableBody)
//            {
//                var tr = new TableRow();

//                foreach (var col in row)
//                {

//                    var colRun = new Run();
//                    ProcessTextWithNewLines(col, colRun);
//                    var cell = new TableCell();
//                    cell.Append(new Paragraph(colRun));
//                    cell.Append(new TableCellProperties(
//                        new TableCellWidth { Type = TableWidthUnitValues.Auto }));
//                    tr.Append(cell);
//                }

//                table.Append(tr);
//            }

//            return table;
//        }

//        private Paragraph GetSectionTableNameParagraph(string tableName)
//        {
//            Paragraph tableNameParagraph = new Paragraph();

//            ParagraphProperties headerParagraphProperties = new ParagraphProperties();


//            Run run = new Run();
//            RunProperties runProperties = new RunProperties();

//            runProperties.Append(new FontSize() { Val = "24" });
//            Text tableNameText = new Text() { Text = tableName.ToUpper() };

//            run.Append(runProperties);
//            run.Append(tableNameText);
//            //headerRun.Append(new Break());
//            tableNameParagraph.Append(headerParagraphProperties);
//            tableNameParagraph.Append(run);

//            return tableNameParagraph;
//        }

//        private Paragraph GetSectionTitleParagraph(string title)
//        {
//            Paragraph sectionTitleParagraph = new Paragraph();

//            ParagraphProperties headerParagraphProperties = new ParagraphProperties();

//            Run run = new Run();
//            RunProperties runProperties = new RunProperties();

//            Text titleText = new Text() { Text = title };

//            run.Append(runProperties);
//            run.Append(titleText);
//            sectionTitleParagraph.Append(headerParagraphProperties);
//            sectionTitleParagraph.Append(run);

//            if (sectionTitleParagraph.Elements<ParagraphProperties>().Count() == 0)
//            {
//                sectionTitleParagraph.PrependChild<ParagraphProperties>(new ParagraphProperties());
//            }

//            ParagraphProperties pPr = sectionTitleParagraph.ParagraphProperties;

//            if (pPr.ParagraphStyleId == null)
//                pPr.ParagraphStyleId = new ParagraphStyleId();

//            pPr.ParagraphStyleId.Val = Constants.SectionHeaderStyle;

//            return sectionTitleParagraph;
//        }

//        private Paragraph GetAdditionalInfoTitleParagraph(string title)
//        {
//            Paragraph additionalInfoTitleParagraph = new Paragraph();

//            ParagraphProperties headerParagraphProperties = new ParagraphProperties();


//            Run run = new Run();
//            RunProperties runProperties = new RunProperties();

//            FontSize headerFontSize = new FontSize();
//            headerFontSize.Val = new DocumentFormat.OpenXml.StringValue("22");

//            runProperties.FontSize = headerFontSize;

//            Text titleText = new Text() { Text = title };

//            run.Append(runProperties);
//            run.Append(titleText);
//            //headerRun.Append(new Break());
//            additionalInfoTitleParagraph.Append(headerParagraphProperties);
//            additionalInfoTitleParagraph.Append(run);

//            if (additionalInfoTitleParagraph.Elements<ParagraphProperties>().Count() == 0)
//            {
//                additionalInfoTitleParagraph.PrependChild<ParagraphProperties>(new ParagraphProperties());
//            }

//            return additionalInfoTitleParagraph;
//        }

//        private Paragraph GetAdditionalInfoTextParagraph(string text)
//        {
//            Paragraph additionalInfoParagraph = new Paragraph();

//            ParagraphProperties paragraphProperties = new ParagraphProperties();

//            additionalInfoParagraph.Append(paragraphProperties);
            
//            //параграф может быть либо с ссылкой, либо с текстом
//            if (text.Contains("</a>"))
//            {
//                AddAdditionalInfoLinkToParagraph(additionalInfoParagraph, text);

//            }
//            else
//            {
//                AddAdditionalInfoTextToParagraph(additionalInfoParagraph, text);
//            }

//            return additionalInfoParagraph;
//        }

//        private void AddAdditionalInfoTextToParagraph(Paragraph additionalInfoParagraph, string text)
//        {

//            Run run = new Run();
//            RunProperties runProperties = new RunProperties();

//            FontSize headerFontSize = new FontSize();
//            headerFontSize.Val = new StringValue("22");

//            runProperties.FontSize = headerFontSize;

//            run.Append(runProperties);

//            //обрабатываем символ переноса на новую строку
//            ProcessTextWithNewLines(text, run);

//            additionalInfoParagraph.Append(run);
//        }

//        private void AddAdditionalInfoLinkToParagraph(Paragraph additionalInfoParagraph, string text)
//        {

//            const string textPartStart = ">";
//            const string textPartFinish = "</";

//            const string hrefValuePartStart = @"href=""";
//            const string hrefValuePartFinish = @""" target";

//            //выделим часть с текстом  
//            string textValuePart = text.Substring(text.IndexOf(textPartStart) + textPartStart.Length, text.IndexOf(textPartFinish) - text.IndexOf(textPartStart) - textPartStart.Length);

//            //выделим ссылку из href
//            string hrefValuePart = text.Substring(text.IndexOf(hrefValuePartStart) + hrefValuePartStart.Length, text.IndexOf(hrefValuePartFinish) - text.IndexOf(hrefValuePartStart) - hrefValuePartStart.Length);

//            var rel = CurrentDoc.MainDocumentPart.AddHyperlinkRelationship(new Uri(hrefValuePart), true);

//            additionalInfoParagraph.Append(
//                new Hyperlink(
//                    new Run(
//                        new RunProperties(
//                            new RunStyle { Val = "Hyperlink", },
//                            new Underline { Val = UnderlineValues.Single },
//                            new Color { ThemeColor = ThemeColorValues.Hyperlink }),
//                        new Text { Text = textValuePart }
//                    ))
//                { History = OnOffValue.FromBoolean(true), Id = rel.Id }

//                );
//        }
     
//        private Paragraph GetOtherAppraisalTypeNameParagraph(string type)
//        {
//            Paragraph otherAppraisalsTypeNameParagraph = new Paragraph();

//            ParagraphProperties headerParagraphProperties = new ParagraphProperties();
//            //headerParagraphProperties.Append(new ParagraphStyleId() { Val = "Normal" });
//            //headerParagraphProperties.Append(new Justification() { Val = JustificationValues.Center });
//            //headerParagraphProperties.Append(new ParagraphMarkRunProperties());

//            Run run = new Run();
//            RunProperties runProperties = new RunProperties();

//            FontSize headerFontSize = new FontSize();
//            headerFontSize.Val = new DocumentFormat.OpenXml.StringValue("22");

//            runProperties.FontSize = headerFontSize;

//            Text titleText = new Text() { Text = type };

//            run.Append(runProperties);
//            run.Append(titleText);
//            //headerRun.Append(new Break());
//            otherAppraisalsTypeNameParagraph.Append(headerParagraphProperties);
//            otherAppraisalsTypeNameParagraph.Append(run);

//            if (otherAppraisalsTypeNameParagraph.Elements<ParagraphProperties>().Count() == 0)
//            {
//                otherAppraisalsTypeNameParagraph.PrependChild<ParagraphProperties>(new ParagraphProperties());
//            }

//            // Get a reference to the ParagraphProperties object.
//            ParagraphProperties pPr = otherAppraisalsTypeNameParagraph.ParagraphProperties;

//            // If a ParagraphStyleId object does not exist, create one.
//            if (pPr.ParagraphStyleId == null)
//                pPr.ParagraphStyleId = new ParagraphStyleId();

//            // Set the style of the paragraph.
//            pPr.ParagraphStyleId.Val = Constants.SectionHeaderStyle;

//            return otherAppraisalsTypeNameParagraph;
//        }

//        private Paragraph GetFeedbackSectionHintValueParagraph(string hint)
//        {
//            Paragraph sectionTitleParagraph = new Paragraph();

//            ParagraphProperties headerParagraphProperties = new ParagraphProperties();

//            Run run = new Run();
//            RunProperties runProperties = new RunProperties();

//            FontSize headerFontSize = new FontSize();
//            headerFontSize.Val = new DocumentFormat.OpenXml.StringValue("22");

//            runProperties.FontSize = headerFontSize;

//            Text titleText = new Text() { Text = hint };

//            run.Append(runProperties);
//            run.Append(titleText);
//            //run.Append(new Break());
//            sectionTitleParagraph.Append(headerParagraphProperties);
//            sectionTitleParagraph.Append(run);

//            return sectionTitleParagraph;
//        }

//        private Paragraph GetFeedbackSectionParticipantNameParagraph(string name, bool hasHint)
//        {


//            Paragraph sectionTitleParagraph = new Paragraph();

//            ParagraphProperties headerParagraphProperties = new ParagraphProperties();

//            if (hasHint)
//                headerParagraphProperties.Indentation = new Indentation() { Left = "300" };

//            Run run = new Run();
//            RunProperties runProperties = new RunProperties();

//            FontSize headerFontSize = new FontSize();
//            headerFontSize.Val = new DocumentFormat.OpenXml.StringValue("22");

//            runProperties.FontSize = headerFontSize;

//            Text titleText = new Text() { Text = name };

//            run.Append(runProperties);
//            run.Append(titleText);
//            //run.Append(new Break());
//            sectionTitleParagraph.Append(headerParagraphProperties);
//            sectionTitleParagraph.Append(run);

//            return sectionTitleParagraph;
//        }

//        private Paragraph GetFeedbackSectionParticipantFeedbackParagraph(string feedback, bool hasHint)
//        {
//            Paragraph sectionTitleParagraph = new Paragraph();

//            ParagraphProperties paragraphProperties = new ParagraphProperties();

//            if (hasHint)
//                paragraphProperties.Indentation = new Indentation() { Left = "600" };
//            else
//                paragraphProperties.Indentation = new Indentation() { Left = "300" };

//            Run run = new Run();
//            RunProperties runProperties = new RunProperties();

//            FontSize headerFontSize = new FontSize();
//            headerFontSize.Val = new DocumentFormat.OpenXml.StringValue("22");

//            runProperties.FontSize = headerFontSize;

//            run.Append(runProperties);

//            //обрабатываем символ переноса на новую строку
//            ProcessTextWithNewLines(feedback, run);

//            //run.Append(new Break());
//            sectionTitleParagraph.Append(paragraphProperties);
//            sectionTitleParagraph.Append(run);

//            return sectionTitleParagraph;
//        }

//        private Paragraph GetSimpleTextParagraph(string text)
//        {
//            Paragraph simpleTextParagraph = new Paragraph();

//            ParagraphProperties paragraphProperties = new ParagraphProperties();

//            Run run = new Run();
//            RunProperties runProperties = new RunProperties();

//            FontSize headerFontSize = new FontSize();
//            headerFontSize.Val = new DocumentFormat.OpenXml.StringValue("22");

//            runProperties.FontSize = headerFontSize;

//            run.Append(runProperties);

//            //обрабатываем символ переноса на новую строку
//            ProcessTextWithNewLines(text, run);

//            simpleTextParagraph.Append(paragraphProperties);
//            simpleTextParagraph.Append(run);

//            return simpleTextParagraph;
//        }


//        private void ProcessTextWithNewLines(string text, Run run)
//        {
//            //чтобы учесть обычные переносы, преобразуем их сначала в html line breaks
//            text = HtmlHelper.ConvertNewLinesToHtmlLineBreaks(text);

//            if (text.Contains(HtmlHelper.LineBreakTag) || text.Contains(HtmlHelper.LineBreakTagUnclosed))
//            {
//                var textItems = text.Split(new[] { HtmlHelper.LineBreakTag, HtmlHelper.LineBreakTagUnclosed }, StringSplitOptions.RemoveEmptyEntries);

//                for (int i = 0; i < textItems.Length; i++)
//                {
//                    Text txt = new Text() { Text = textItems[i] };
//                    run.Append(txt);
//                    if (i != textItems.Length - 1)
//                        run.Append(new Break());
//                }
//            }
//            else
//            {
//                Text titleText = new Text() { Text = text };
//                run.Append(titleText);
//            }
//        }
//        public void CreateAndAddHeaderParagraphStyle(StyleDefinitionsPart styleDefinitionsPart,
//                string styleid, string stylename, string aliases = "")
//        {
//            // Access the root element of the styles part.
//            Styles styles = styleDefinitionsPart.Styles;
//            if (styles == null)
//            {
//                styleDefinitionsPart.Styles = new Styles();
//                styleDefinitionsPart.Styles.Save();
//            }

//            // Create a new paragraph style element and specify some of the attributes.
//            Style style = new Style()
//            {
//                Type = StyleValues.Paragraph,
//                StyleId = styleid,
//                CustomStyle = true,
//                Default = false
//            };

//            // Create and add the child elements (properties of the style).
//            //Aliases aliases1 = new Aliases() { Val = aliases };
//            AutoRedefine autoredefine1 = new AutoRedefine() { Val = OnOffOnlyValues.Off };
//            //BasedOn basedon1 = new BasedOn() { Val = "Normal" };
//            //LinkedStyle linkedStyle1 = new LinkedStyle() { Val = "OverdueAmountChar" };
//            Locked locked1 = new Locked() { Val = OnOffOnlyValues.Off };
//            PrimaryStyle primarystyle1 = new PrimaryStyle() { Val = OnOffOnlyValues.On };
//            StyleHidden stylehidden1 = new StyleHidden() { Val = OnOffOnlyValues.Off };
//            SemiHidden semihidden1 = new SemiHidden() { Val = OnOffOnlyValues.Off };
//            StyleName styleName1 = new StyleName() { Val = stylename };
//            //NextParagraphStyle nextParagraphStyle1 = new NextParagraphStyle() { Val = "Normal" };
//            UIPriority uipriority1 = new UIPriority() { Val = 1 };
//            UnhideWhenUsed unhidewhenused1 = new UnhideWhenUsed() { Val = OnOffOnlyValues.On };
//            //if (aliases != "")
//            //    style.Append(aliases1);
//            style.Append(autoredefine1);
//            //style.Append(basedon1);
//            //style.Append(linkedStyle1);
//            style.Append(locked1);
//            style.Append(primarystyle1);
//            style.Append(stylehidden1);
//            style.Append(semihidden1);
//            style.Append(styleName1);
//            //style.Append(nextParagraphStyle1);
//            style.Append(uipriority1);
//            style.Append(unhidewhenused1);

//            // Create the StyleRunProperties object and specify some of the run properties.
//            StyleRunProperties styleRunProperties1 = new StyleRunProperties();
//            //Bold bold1 = new Bold();
//            //Color color1 = new Color() { ThemeColor = ThemeColorValues.Accent2 };
//            //RunFonts font1 = new RunFonts() { Ascii = "Calibri Light" };
//            //Italic italic1 = new Italic();
//            // Specify a 12 point size.
//            FontSize fontSize1 = new FontSize() { Val = "28" };
//            //styleRunProperties1.Append(bold1);
//            // styleRunProperties1.Append(color1);
//            //styleRunProperties1.Append(font1);
//            styleRunProperties1.Append(fontSize1);
//            //styleRunProperties1.Append(italic1);

//            // Add the run properties to the style.
//            style.Append(styleRunProperties1);

//            // Add the style to the styles part.
//            styles.Append(style);
//        }

//        private TableProperties GetBaseTableProperties()
//        {
//            TableProperties props = new TableProperties(
//                new TableBorders(
//                new TopBorder
//                {
//                    Val = new EnumValue<BorderValues>(BorderValues.Single),
//                    Size = 6
//                },
//                new BottomBorder
//                {
//                    Val = new EnumValue<BorderValues>(BorderValues.Single),
//                    Size = 6
//                },
//                new LeftBorder
//                {
//                    Val = new EnumValue<BorderValues>(BorderValues.Single),
//                    Size = 6
//                },
//                new RightBorder
//                {
//                    Val = new EnumValue<BorderValues>(BorderValues.Single),
//                    Size = 6
//                },
//                new InsideHorizontalBorder
//                {
//                    Val = new EnumValue<BorderValues>(BorderValues.Single),
//                    Size = 6
//                },
//                new InsideVerticalBorder
//                {
//                    Val = new EnumValue<BorderValues>(BorderValues.Single),
//                    Size = 6
//                }));

//            TableWidth tableWidth = new TableWidth
//            {
//                Type = TableWidthUnitValues.Pct,
//                Width = "5000"
//            };

//            props.Append(tableWidth);
//            return props;
//        }


//        // Add a StylesDefinitionsPart to the document.  Returns a reference to it.
//        public static StyleDefinitionsPart AddStylesPartToPackage(WordprocessingDocument doc)
//        {
//            StyleDefinitionsPart part;
//            part = doc.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
//            Styles root = new Styles();
//            root.Save(part);
//            return part;
//        }



//    }
//}
