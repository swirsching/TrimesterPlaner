using Svg;
using System.Windows.Media;
using TrimesterPlaner.Extensions;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.Models
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

        private record HeightsType(int Fehlerteam, int Burndown, int Header, int Developer, int Vacation);
        private HeightsType Heights { get; } = new(200, 500, 40, 60, 20);

        private record WidthsType(int WeekDay, int WeekEndDay, int Left);
        private WidthsType Widths { get; } = new(50, 5, 40);

        private record MarginsType(int Plan, int BurndownVertical);
        private MarginsType Margins { get; } = new(1, 50);

        private record TextColorsType(SvgColourServer Default, SvgColourServer Vacation);
        private record BurndownColorsType(SvgColourServer Axis, SvgColourServer Grid, SvgColourServer Capacity, SvgColourServer Total, SvgColourServer Promised);
        private record StartEndBorderColors(SvgColourServer Start, SvgColourServer End, SvgColourServer Border);
        private record ColorsType(
            TextColorsType Text,
            SvgColourServer BadArea, 
            BurndownColorsType BurnDown,
            SvgColourServer Weekend, 
            StartEndBorderColors Ticket, 
            StartEndBorderColors Bug, 
            StartEndBorderColors Special,
            SvgColourServer Vacation);
        private ColorsType Colors { get; } = new(
            Text: new(new(IvuColors.GRAY10), new(IvuColors.WHITE)),
            BadArea: new(IvuColors.RED60),
            BurnDown: new(new(IvuColors.BLACK), new(IvuColors.GRAY40), new(IvuColors.BLACK), new(IvuColors.PRIMARY_CYAN), new(IvuColors.BLUE20)),
            Weekend: new(IvuColors.GRAY70),
            Ticket: new(new(IvuColors.CYAN70), new(IvuColors.PRIMARY_CYAN), new(IvuColors.CYAN20)),
            Bug: new(new(IvuColors.MAIGREEN70), new(IvuColors.PRIMARY_GREEN), new(IvuColors.GREEN20)),
            Special: new(new(IvuColors.ORANGE70), new(IvuColors.ORANGE40), new(IvuColors.ORANGE20)),
            Vacation: new(IvuColors.BLUE20));

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
            int width = Widths.Left;
            foreach (var week in data.Weeks)
            {
                foreach (var day in week.Days)
                {
                    day.X = width;
                    width += day.Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ? Widths.WeekEndDay : Widths.WeekDay;
                }
            }

            int plannedWidth = (from week in data.Weeks
                                from day in week.Days
                                where !day.IsBadArea
                                select day.X).Last();

            int fehlerteamHeight = data.Fehlerteam ? Heights.Fehlerteam + Margins.BurndownVertical : 0;
            int burndownHeight = data.Burndown ? Heights.Burndown + Margins.BurndownVertical : 0;
            int planHeight = Heights.Header + data.Developers.Count() * Heights.Developer;
            int height = fehlerteamHeight + burndownHeight + planHeight;
            SvgDocument document = new()
            {
                Width = width,
                Height = height,
                ViewBox = new SvgViewBox(0, 0, width, height),
            };

            document.Children.Add(DefineGradients());
            if (width > plannedWidth)
            {
                document.Children.Add(GenerateBadArea(width - plannedWidth).Translate(plannedWidth, 0));
            }
            if (data.Fehlerteam)
            {
                document.Children.Add(GenerateFehlerteam(data, width));
            }
            if (data.Burndown)
            {
                document.Children.Add(GenerateBurndown(data, width).Translate(0, fehlerteamHeight));
            }
            document.Children.Add(GenerateTrimester(data, width).Translate(0, fehlerteamHeight + burndownHeight));

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

        private SvgGroup GenerateBadArea(int width)
        {
            SvgGroup group = new();

            group.Children.Add(new SvgRectangle()
            {
                Width = width,
                Height = new SvgUnit(SvgUnitType.Percentage, 100),
                Fill = Colors.BadArea,
            });

            return group;
        }

        private SvgGroup GenerateFehlerteam(PreparedData data, int width)
        {
            SvgGroup group = new();
            group.Children.Add(new SvgRectangle()
            {
                Width = width,
                Height = Heights.Fehlerteam,
                Fill = Colors.Weekend,
            });
            return group;
        }

        private SvgGroup GenerateBurndown(PreparedData data, int width)
        {
            SvgGroup group = new();
            group.Children.Add(MakeAxis(width, Heights.Burndown).Translate(Widths.Left, 0));
            
            List<Point> capacityPoints = [], totalPoints = [], promisedPoints = [];
            foreach (Week week in data.Weeks)
            {
                foreach (Day day in week.Days)
                {
                    capacityPoints.Add(new Point(day.X, day.Capacity));
                    totalPoints.Add(new Point(day.X, day.Total));
                    promisedPoints.Add(new Point(day.X, day.Promised));
                }
            }
            double maxValue = (from point in capacityPoints.Concat(totalPoints).Concat(promisedPoints)
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
            group.Children.Add(MakeLine(promisedPoints, roundedMaxValue, pxPerValue, Colors.BurnDown.Promised, 2));

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
                    int x = day.X + Widths.WeekDay / 2;
                    if (day.Date.DayOfWeek is DayOfWeek.Monday or DayOfWeek.Friday)
                    {
                        group.Children.Add(MakeText(day.Date.ToString("dd.MM."), SvgTextAnchor.Middle, FontSizes.Small).Translate(x, Heights.Header / 2).Rotate(day.Date.DayOfWeek == DayOfWeek.Monday ? -90 : 90));
                    }
                    if (day.Date.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        group.Children.Add(MakeText($"KW {week.Weeknum}", SvgTextAnchor.Middle, FontSizes.Big).Translate(x, Heights.Header / 2));
                    }
                }
            }

            return group;
        }

        private SvgGroup GenerateDeveloper(DeveloperData developer)
        { 
            SvgGroup group = new();

            group.Children.Add(new SvgRectangle()
            { 
                Width = Widths.Left,
                Height = Heights.Developer,
                Fill = Colors.Weekend,
            });
            group.Children.Add(MakeText(developer.Abbreviation, SvgTextAnchor.Middle, FontSizes.Medium).Translate(Widths.Left * 2 / 3, Heights.Developer / 2).Rotate(-90));

            foreach (Day freeDay in developer.FreeDays)
            {
                group.Children.Add(new SvgRectangle()
                { 
                    Width = freeDay.Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ? Widths.WeekEndDay : Widths.WeekDay,
                    Height = Heights.Developer,
                    Fill = Colors.Weekend,
                }.Translate(freeDay.X, 0));
            }

            group.Children.Add(GeneratePlans(developer.Plans));
            group.Children.Add(GenerateVacations(developer.Vacations));

            return group;
        }

        private SvgGroup GeneratePlans(IEnumerable<PlanData> plans)
        {
            SvgGroup group = new();

            foreach (PlanData plan in plans)
            {
                group.Children.Add(GeneratePlan(plan).Translate(plan.RemainingPerDay.First().Key.X, 0));
            }

            return group;
        }

        private SvgGroup GeneratePlan(PlanData plan)
        {
            SvgGroup group = new();

            int width = plan.RemainingPerDay.Last().Key.X - plan.RemainingPerDay.First().Key.X + Widths.WeekDay;
            int innerWidth = width - 2 * Margins.Plan;
            group.Children.Add(new SvgRectangle()
            { 
                CornerRadiusX = CornerRadius,
                CornerRadiusY = CornerRadius,
                Width = innerWidth,
                Height = Heights.Developer - 2 * Margins.Plan,
                Fill = plan.PlanType switch
                {
                    PlanType.Ticket => TicketGradient,
                    PlanType.Bug => BugGradient,
                    PlanType.Special => SpecialGradient,
                    _ => throw new NotImplementedException(),
                },
                Stroke = Colors.Ticket.Border,
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
                group.Children.Add(svgText.Translate(width / 2, Heights.Developer / 3));
            }
            if (!string.IsNullOrEmpty(plan.SecondRow))
            {
                group.Children.Add(MakeText(plan.SecondRow, SvgTextAnchor.Middle, FontSizes.Medium, innerWidth).Translate(width / 2, Heights.Developer * 2 / 3));
            }
            if (!string.IsNullOrEmpty(plan.TopLeft))
            {
                group.Children.Add(MakeText(plan.TopLeft, SvgTextAnchor.Start, FontSizes.Small, innerWidth).Translate(2 * Margins.Plan, Heights.Developer / 5));
            }

            return group.Translate(Margins.Plan, Margins.Plan);
        }

        private SvgGroup GenerateVacations(IEnumerable<VacationData> vacations)
        {
            SvgGroup group = new();

            foreach (VacationData vacation in vacations)
            {
                if (vacation.Days.Any())
                {
                    group.Children.Add(GenerateVacation(vacation).Translate(vacation.Days.First().X, 0));
                }
            }

            return group;
        }

        private SvgGroup GenerateVacation(VacationData vacation) 
        { 
            SvgGroup group = new();

            var lastDay = vacation.Days.Last();
            int width = lastDay.X - vacation.Days.First().X + (lastDay.Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ? Widths.WeekEndDay : Widths.WeekDay);
            group.Children.Add(new SvgRectangle()
            {
                CornerRadiusX = CornerRadius,
                CornerRadiusY = CornerRadius,
                Width = width,
                Height = Heights.Vacation,
                Fill = Colors.Vacation,
            });
            group.Children.Add(MakeText(vacation.Label, SvgTextAnchor.Middle, FontSizes.Small, width, Colors.Text.Vacation).Translate(width / 2, Heights.Vacation / 2));

            return group;
        }
    }
}