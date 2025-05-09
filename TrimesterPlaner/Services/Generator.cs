﻿using Svg;
using TrimesterPlaner.Extensions;
using Utilities.Utilities;

namespace TrimesterPlaner.Services
{
    public interface IGenerator
    {
        public SvgDocument? Generate(PreparedData data);
    }

    public class Generator : IGenerator
    {
        public Generator()
        {
            TicketGradient = MakeHorizontalLinearGradient("ticket_grad", Colors.Ticket.Start, Colors.Ticket.End);
            BugGradient = MakeHorizontalLinearGradient("bug_grad", Colors.Bug.Start, Colors.Bug.End);
            SpecialGradient = MakeHorizontalLinearGradient("special_grad", Colors.Special.Start, Colors.Special.End);
        }

        private SvgLinearGradientServer TicketGradient { get; }
        private SvgLinearGradientServer BugGradient { get; }
        private SvgLinearGradientServer SpecialGradient { get; }

        private string FontFamily { get; } = "Calibri";

        private record FontSizesType(int Small, int Medium, int Big);
        private FontSizesType FontSizes { get; } = new(12, 15, 20);

        private record HeightsType(int Burndown, int Header, int Developer, int Remaining, int Vacation, int DateRow);
        private HeightsType Heights { get; } = new(500, 40, 70, 5, 20, 60);

        private record LineMarginsType(int Offset, int TextMargin);
        private record MarginsType(int Plan, LineMarginsType Lines, int BurndownVertical);
        private MarginsType Margins { get; } = new(2, new(2, 20), 50);

        private record TextColorsType(SvgColourServer Default, SvgColourServer Vacation);
        private record BurndownColorsType(SvgColourServer Axis, SvgColourServer Grid, SvgColourServer Capacity, SvgColourServer Total);
        private record StartEndBorderColors(SvgColourServer Start, SvgColourServer End, SvgColourServer Border);
        private record LinesColors(SvgColourServer Today, SvgColourServer Entwicklungsstart, SvgColourServer Entwicklungsschluss);
        private record ColorsType(
            TextColorsType Text,
            SvgColourServer BadArea,
            BurndownColorsType BurnDown,
            SvgColourServer Weekend,
            StartEndBorderColors Ticket,
            StartEndBorderColors Bug,
            StartEndBorderColors Special,
            SvgColourServer Remaining,
            SvgColourServer Vacation,
            LinesColors Lines);
        private ColorsType Colors { get; } = new(
            Text: new(new(IvuColors.GRAY10), new(IvuColors.WHITE)),
            BadArea: new(IvuColors.RED60),
            BurnDown: new(new(IvuColors.BLACK), new(IvuColors.GRAY40), new(IvuColors.BLACK), new(IvuColors.PRIMARY_CYAN)),
            Weekend: new(IvuColors.GRAY70),
            Ticket: new(new(IvuColors.CYAN70), new(IvuColors.PRIMARY_CYAN), new(IvuColors.CYAN20)),
            Bug: new(new(IvuColors.MAIGREEN70), new(IvuColors.PRIMARY_GREEN), new(IvuColors.GREEN20)),
            Special: new(new(IvuColors.ORANGE70), new(IvuColors.ORANGE40), new(IvuColors.ORANGE20)),
            Remaining: new(IvuColors.GREEN40),
            Vacation: new(IvuColors.BLUE20),
            Lines: new(new(IvuColors.GREEN30), new(IvuColors.RED40), new(IvuColors.RED40)));

        private int CornerRadius { get; } = 5;
        private int BurnDownValueStepSize { get; } = 20;

        private static SvgLinearGradientServer MakeHorizontalLinearGradient(string id, SvgColourServer startColor, SvgColourServer endColor)
        {
            SvgLinearGradientServer gradient = new()
            {
                ID = id,
                X1 = new SvgUnit(SvgUnitType.Percentage, 0),
                Y1 = new SvgUnit(SvgUnitType.Percentage, 0),
                X2 = new SvgUnit(SvgUnitType.Percentage, 100),
                Y2 = new SvgUnit(SvgUnitType.Percentage, 0)
            };

            gradient.Children.Add(new SvgGradientStop
            {
                Offset = 0,
                StopColor = startColor,
            });

            gradient.Children.Add(new SvgGradientStop
            {
                Offset = 1,
                StopColor = endColor,
            });
            return gradient;
        }

        private SvgText MakeText(string text, SvgTextAnchor anchor, int fontSize, int? maxWidth = null, SvgColourServer? color = null)
        {
            return new SvgText(text.Shorten(FontFamily, fontSize, maxWidth))
            {
                TextAnchor = anchor,
                FontFamily = FontFamily,
                FontSize = fontSize,
                Fill = color ?? Colors.Text.Default,
            }.Translate(0, fontSize / 3);
        }

        private static SvgPolyline MakeLine(SvgPointCollection points, SvgPaintServer color, int width = 1) => new()
        {
            Points = points,
            Stroke = color,
            Fill = SvgPaintServer.None,
            StrokeWidth = width,
        };

        private SvgPolyline MakeAxis(int width, int height) => MakeLine(
        [
            0, 0,
            0, height,
            width, height,
        ], Colors.BurnDown.Axis);

        private record Point(int X, double Y);
        private static SvgPolyline MakeLine(IEnumerable<Point> points, double maxValue, double pxPerValue, SvgPaintServer color, int width = 1)
        {
            SvgPointCollection pointCollection = [];
            foreach (Point point in points)
            {
                pointCollection.Add(point.X);
                pointCollection.Add((float)((maxValue - point.Y) * pxPerValue));
            }
            return MakeLine(pointCollection, color, width);
        }

        public SvgDocument? Generate(PreparedData data)
        {
            int width = (from week in data.Weeks
                         from day in week.Days
                         select day).Last().GetX(1);
            int plannedWidth = (from week in data.Weeks
                                from day in week.Days
                                where !day.IsBadArea
                                select day).Last().GetX(1);

            int burndownHeight = data.Burndown ? Heights.Burndown + Margins.BurndownVertical : 0;
            int planHeight = Heights.Header + data.Developers.Count() * Heights.Developer;
            int height = burndownHeight + planHeight + Heights.DateRow;
            SvgDocument document = new()
            {
                Width = width,
                Height = height,
                ViewBox = new SvgViewBox(0, 0, width, height),
            };

            document.Children.Add(DefineGradients());
            if (width > plannedWidth)
            {
                document.Children.Add(GenerateBadArea(width - plannedWidth, height - Heights.DateRow).Translate(plannedWidth, 0));
            }
            if (data.Burndown)
            {
                document.Children.Add(GenerateBurndown(data, width));
            }
            document.Children.Add(GenerateTrimester(data, width).Translate(0, burndownHeight));

            document.Children.Add(GenerateLines(data, height));

            return document;
        }

        private SvgDefinitionList DefineGradients()
        {
            SvgDefinitionList definitions = new();
            definitions.Children.Add(TicketGradient);
            definitions.Children.Add(BugGradient);
            definitions.Children.Add(SpecialGradient);
            return definitions;
        }

        private SvgGroup GenerateBadArea(int width, int height)
        {
            SvgGroup group = new();

            group.Children.Add(new SvgRectangle()
            {
                Width = width,
                Height = height,
                Fill = Colors.BadArea,
            });

            return group;
        }

        private SvgGroup GenerateBurndown(PreparedData data, int width)
        {
            SvgGroup group = new();
            group.Children.Add(MakeAxis(width, Heights.Burndown).Translate(Widths.Left, 0));

            List<Point> capacityPoints = [], totalPoints = [];
            foreach (Week week in data.Weeks)
            {
                foreach (Day day in week.Days)
                {
                    capacityPoints.Add(new Point(day.GetX(0), day.Capacity));
                    totalPoints.Add(new Point(day.GetX(0), day.Total));
                }
            }
            double maxValue = (from point in capacityPoints.Concat(totalPoints)
                               select point.Y).Max();
            double roundedMaxValue = BurnDownValueStepSize * Math.Round(maxValue / BurnDownValueStepSize, 0, MidpointRounding.ToPositiveInfinity) + BurnDownValueStepSize / 2;
            double pxPerValue = Heights.Burndown / roundedMaxValue;

            for (int value = 0; value < roundedMaxValue; value += BurnDownValueStepSize)
            {
                float y = (float)((roundedMaxValue - value) * pxPerValue);
                group.Children.Add(MakeText($"{value}", SvgTextAnchor.Middle, FontSizes.Small).Translate(Widths.Left / 2, y));
                group.Children.Add(new SvgLine()
                {
                    StartX = Widths.Left,
                    EndX = width,
                    StartY = y,
                    EndY = y,
                    Stroke = Colors.BurnDown.Grid,
                    StrokeWidth = 0.2f,
                });
            }

            group.Children.Add(MakeLine(capacityPoints, roundedMaxValue, pxPerValue, Colors.BurnDown.Capacity));
            group.Children.Add(MakeLine(totalPoints, roundedMaxValue, pxPerValue, Colors.BurnDown.Total, 2));

            return group;
        }

        private SvgGroup GenerateTrimester(PreparedData data, int width)
        {
            SvgGroup group = new();

            group.Children.Add(GenerateHeader(data.Weeks, width));
            float y = Heights.Header;
            foreach (DeveloperData developer in data.Developers)
            {
                group.Children.Add(GenerateDeveloper(developer).Translate(0, y));
                y += Heights.Developer;
            }

            return group;
        }

        private SvgGroup GenerateHeader(IEnumerable<Week> weeks, int width)
        {
            SvgGroup group = new();

            group.Children.Add(new SvgRectangle()
            {
                Width = width,
                Height = Heights.Header,
                Fill = Colors.Weekend,
            });

            foreach (Week week in weeks)
            {
                foreach (Day day in week.Days)
                {
                    if (day.Date.DayOfWeek is DayOfWeek.Monday or DayOfWeek.Friday)
                    {
                        group.Children.Add(MakeText(day.Date.ToString("dd.MM."), SvgTextAnchor.Middle, FontSizes.Small).Translate(day.GetX(0.5), Heights.Header / 2).Rotate(day.Date.DayOfWeek == DayOfWeek.Monday ? -90 : 90));
                    }
                    else if (day.Date.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        group.Children.Add(MakeText($"KW {week.Weeknum}", SvgTextAnchor.Middle, FontSizes.Big).Translate(day.GetX(0.5), Heights.Header / 2));
                    }
                }
            }

            return group;
        }

        private SvgGroup GenerateDeveloper(DeveloperData developer)
        {
            SvgGroup group = new();

            group.Children.Add(GenerateDeveloperAbbr(developer.Abbreviation));

            foreach (Day freeDay in developer.FreeDays)
            {
                group.Children.Add(new SvgRectangle()
                {
                    Width = freeDay.IsWeekend() ? Widths.WeekEndDay : Widths.WeekDay,
                    Height = Heights.Developer,
                    Fill = Colors.Weekend,
                }.Translate(freeDay.GetX(0), 0));
            }

            group.Children.Add(GeneratePlans(developer.Plans));
            group.Children.Add(GenerateVacations(developer.Vacations));

            return group;
        }

        private SvgGroup GenerateDeveloperAbbr(string abbr)
        {
            SvgGroup group = new();

            group.Children.Add(new SvgRectangle()
            {
                Width = Widths.Left,
                Height = Heights.Developer,
                Fill = Colors.Weekend,
            });
            group.Children.Add(MakeText(abbr, SvgTextAnchor.Middle, FontSizes.Medium).Translate(Widths.Left * 2 / 3, Heights.Developer / 2).Rotate(-90));

            return group;
        }

        private SvgGroup GeneratePlans(IEnumerable<PlanData> plans)
        {
            SvgGroup group = new();

            foreach (PlanData plan in plans)
            {
                group.Children.Add(GeneratePlan(plan, plan.StartX, plan.EndX, plan.RemainingX));
            }

            return group;
        }

        private SvgGroup GeneratePlan(PlanData plan, int startX, int endX, int remainingX)
        {
            SvgGroup group = new();

            int width = endX - startX;
            int innerWidth = width - 2 * Margins.Plan;
            int innerHeight = Heights.Developer - 2 * Margins.Plan;
            group.Children.Add(new SvgRectangle
            {
                CornerRadiusX = CornerRadius,
                CornerRadiusY = CornerRadius,
                Width = innerWidth,
                Height = innerHeight,
                Fill = plan.PlanType switch
                {
                    PlanType.Ticket => TicketGradient,
                    PlanType.Bug => BugGradient,
                    PlanType.Special => SpecialGradient,
                    _ => throw new NotImplementedException(),
                },
                Stroke = plan.PlanType switch
                {
                    PlanType.Ticket => Colors.Ticket.Border,
                    PlanType.Bug => Colors.Bug.Border,
                    PlanType.Special => Colors.Special.Border,
                    _ => throw new NotImplementedException(),
                },
            });

            if (!string.IsNullOrEmpty(plan.FirstRow))
            {
                SvgText? svgText;
                if (plan.FirstRow.IsShortEnough(FontFamily, FontSizes.Big, innerWidth))
                {
                    svgText = MakeText(plan.FirstRow, SvgTextAnchor.Middle, FontSizes.Big);
                }
                else
                {
                    svgText = MakeText(plan.FirstRow[4..], SvgTextAnchor.Middle, FontSizes.Medium);
                }
                group.Children.Add(svgText.Translate(width / 2, Heights.Developer / 2));
            }
            if (!string.IsNullOrEmpty(plan.SecondRow))
            {
                group.Children.Add(MakeText(plan.SecondRow, SvgTextAnchor.Middle, FontSizes.Medium, innerWidth).Translate(width / 2, Heights.Developer * 3 / 4));
            }
            if (!string.IsNullOrEmpty(plan.TopLeft))
            {
                group.Children.Add(MakeText(plan.TopLeft, SvgTextAnchor.Start, FontSizes.Small, innerWidth).Translate(2 * Margins.Plan, Heights.Developer / 6));
            }

            if (startX != remainingX)
            {
                group.Children.Add(new SvgRectangle
                {
                    CornerRadiusX = CornerRadius,
                    CornerRadiusY = CornerRadius,
                    Width = remainingX - startX - 2 * Margins.Plan,
                    Height = Heights.Remaining,
                    Fill = Colors.Remaining,
                }.Translate(0, innerHeight - Heights.Remaining));
            }

            SvgGroup outerGroup;
            if (plan.PlanType == PlanType.Ticket)
            {
                outerGroup = new();
                SvgAnchor anchor = new()
                {
                    Href = $"https://confluence.ivu.de/jira/browse/{plan.FirstRow}",
                    Target = "_blank",
                };
                anchor.Children.Add(group);
                outerGroup.Children.Add(anchor);
            }
            else
            {
                outerGroup = group;
            }
            return outerGroup.Translate(Margins.Plan, Margins.Plan).Translate(startX, 0);
        }

        private SvgGroup GenerateVacations(IEnumerable<VacationData> vacations)
        {
            SvgGroup group = new();

            foreach (VacationData vacation in vacations)
            {
                if (vacation.Days.Any())
                {
                    group.Children.Add(GenerateVacation(vacation).Translate(vacation.Days.First().GetX(0), 0));
                }
            }

            return group;
        }

        private SvgGroup GenerateVacation(VacationData vacation)
        {
            SvgGroup group = new();

            var lastDay = vacation.Days.Last();
            int width = lastDay.GetX(1) - vacation.Days.First().GetX(0);
            group.Children.Add(new SvgRectangle()
            {
                CornerRadiusX = CornerRadius,
                CornerRadiusY = CornerRadius,
                Width = width,
                Height = Heights.Vacation,
                Fill = Colors.Vacation,
            });
            group.Children.Add(MakeText(vacation.Label, SvgTextAnchor.Middle, FontSizes.Small, width, Colors.Text.Vacation).Translate(width / 2, Heights.Vacation / 2));

            return group.Translate(0, Margins.Plan);
        }

        private SvgGroup GenerateLines(PreparedData data, int height)
        {
            SvgGroup group = new();

            group.Children.Add(GenerateLine(data.Weeks.GetX(DateTime.Now, PositionInDay.Front) + Margins.Lines.Offset, height, Colors.Lines.Today, MakeLabel("Heute", DateTime.Now), Heights.DateRow / 3, false));
            group.Children.Add(GenerateLine(data.Weeks.GetX(data.Entwicklungsstart, PositionInDay.Front) - Margins.Lines.Offset, height, Colors.Lines.Entwicklungsstart, MakeLabel("Entwicklungsstart", data.Entwicklungsstart), Heights.DateRow * 2 / 3, false));
            group.Children.Add(GenerateLine(data.Weeks.GetX(data.Entwicklungsschluss, PositionInDay.Back) - Margins.Lines.Offset, height, Colors.Lines.Entwicklungsschluss, MakeLabel("Entwicklungsschluss", data.Entwicklungsschluss), Heights.DateRow * 2 / 3, true));
            group.Children.Add(GenerateDeveloperAbbrLabels(data.Developers, Colors.Text.Default, data.Weeks.GetX(DateTime.Now, PositionInDay.Front) - Margins.Lines.Offset));

            return group;
        }

        private static string MakeLabel(string label, DateTime date)
        {
            return $"{label}: {date:dd.MM.yyyy}";
        }

        private SvgGroup GenerateLine(int x, int height, SvgColourServer color, string label, int y, bool isEndLine)
        {
            SvgGroup group = new();

            group.Children.Add(new SvgLine()
            {
                StartX = x,
                EndX = x,
                StartY = 0,
                EndY = height,
                Stroke = color,
                StrokeWidth = 2,
            });

            group.Children.Add(MakeText(label, isEndLine ? SvgTextAnchor.End : SvgTextAnchor.Start, FontSizes.Medium, null, color).Translate(x + (isEndLine ? -1 : 1) * Margins.Lines.TextMargin, height - Heights.DateRow + y));

            return group;
        }

        private SvgGroup GenerateDeveloperAbbrLabels(IEnumerable<DeveloperData> developers, SvgColourServer color, int x)
        {
            SvgGroup group = new();

            int y = Heights.Header + Heights.Vacation / 2;
            foreach (var developer in developers)
            {
                group.Children.Add(MakeText(developer.Abbreviation, SvgTextAnchor.End, FontSizes.Small, null, color).Translate(x, y));
                y += Heights.Developer;
            }

            return group;
        }
    }
}