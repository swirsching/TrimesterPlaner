using System.Drawing;

namespace TrimesterPlaner.Utilities
{
    public static class IvuColors
    {
        private static ColorConverter ColorConverter { get; } = new();

#pragma warning disable CS8605 // Unboxing a possibly null value.
        public static Color BLACK { get; } = (Color)ColorConverter.ConvertFromString("#000000");
        public static Color WHITE { get; } = (Color)ColorConverter.ConvertFromString("#FFFFFF");

        public static Color GRAY10 { get; } = (Color)ColorConverter.ConvertFromString("#18191A");
        public static Color GRAY20 { get; } = (Color)ColorConverter.ConvertFromString("#2A2C2D");
        public static Color GRAY30 { get; } = (Color)ColorConverter.ConvertFromString("#3D3E40");
        public static Color GRAY40 { get; } = (Color)ColorConverter.ConvertFromString("#797C80");
        public static Color GRAY50 { get; } = (Color)ColorConverter.ConvertFromString("#B5BABF");
        public static Color GRAY60 { get; } = (Color)ColorConverter.ConvertFromString("#C8CDD2");
        public static Color GRAY70 { get; } = (Color)ColorConverter.ConvertFromString("#DAE0E5");
        public static Color GRAY80 { get; } = (Color)ColorConverter.ConvertFromString("#E6EAED");
        public static Color GRAY90 { get; } = (Color)ColorConverter.ConvertFromString("#F5F6F7");

        public static Color RED10 { get; } = (Color)ColorConverter.ConvertFromString("#3B0300");
        public static Color RED20 { get; } = (Color)ColorConverter.ConvertFromString("#5B0500");
        public static Color RED30 { get; } = (Color)ColorConverter.ConvertFromString("#990600");
        public static Color RED40 { get; } = (Color)ColorConverter.ConvertFromString("#D40C00");
        public static Color RED50 { get; } = (Color)ColorConverter.ConvertFromString("#DF443A");
        public static Color RED60 { get; } = (Color)ColorConverter.ConvertFromString("#EA7C75");
        public static Color RED70 { get; } = (Color)ColorConverter.ConvertFromString("#EF9C96");
        public static Color RED80 { get; } = (Color)ColorConverter.ConvertFromString("#F4BDB9");
        public static Color RED90 { get; } = (Color)ColorConverter.ConvertFromString("#F9DDDB");

        public static Color ORANGE10 { get; } = (Color)ColorConverter.ConvertFromString("#3B1F00");
        public static Color ORANGE20 { get; } = (Color)ColorConverter.ConvertFromString("#5B3100");
        public static Color ORANGE30 { get; } = (Color)ColorConverter.ConvertFromString("#925008");
        public static Color ORANGE40 { get; } = (Color)ColorConverter.ConvertFromString("#E68003");
        public static Color ORANGE50 { get; } = (Color)ColorConverter.ConvertFromString("#EC9F3F");
        public static Color ORANGE60 { get; } = (Color)ColorConverter.ConvertFromString("#F3BE7D");
        public static Color ORANGE70 { get; } = (Color)ColorConverter.ConvertFromString("#F5CE9C");
        public static Color ORANGE80 { get; } = (Color)ColorConverter.ConvertFromString("#F8DEBD");
        public static Color ORANGE90 { get; } = (Color)ColorConverter.ConvertFromString("#FBEEDD");

        public static Color YELLOW10 { get; } = (Color)ColorConverter.ConvertFromString("#423100");
        public static Color YELLOW20 { get; } = (Color)ColorConverter.ConvertFromString("#664C00");
        public static Color YELLOW30 { get; } = (Color)ColorConverter.ConvertFromString("#AA8700");
        public static Color YELLOW40 { get; } = (Color)ColorConverter.ConvertFromString("#FCDF19");
        public static Color YELLOW50 { get; } = (Color)ColorConverter.ConvertFromString("#FCE651");
        public static Color YELLOW60 { get; } = (Color)ColorConverter.ConvertFromString("#FDEE8B");
        public static Color YELLOW70 { get; } = (Color)ColorConverter.ConvertFromString("#FCF2A7");
        public static Color YELLOW80 { get; } = (Color)ColorConverter.ConvertFromString("#FDF6C4");
        public static Color YELLOW90 { get; } = (Color)ColorConverter.ConvertFromString("#FDFAE1");

        public static Color MAIGREEN10 { get; } = (Color)ColorConverter.ConvertFromString("#2F3104");
        public static Color MAIGREEN20 { get; } = (Color)ColorConverter.ConvertFromString("#494D07");
        public static Color MAIGREEN30 { get; } = (Color)ColorConverter.ConvertFromString("#787C0D");
        public static Color MAIGREEN40 { get; } = (Color)ColorConverter.ConvertFromString("#C8D400");
        public static Color MAIGREEN50 { get; } = (Color)ColorConverter.ConvertFromString("#D5DF3A");
        public static Color MAIGREEN60 { get; } = (Color)ColorConverter.ConvertFromString("#E3EA75");
        public static Color MAIGREEN70 { get; } = (Color)ColorConverter.ConvertFromString("#E9EF96");
        public static Color MAIGREEN80 { get; } = (Color)ColorConverter.ConvertFromString("#F0F4B9");
        public static Color MAIGREEN90 { get; } = (Color)ColorConverter.ConvertFromString("#F7F9DB");

        public static Color GREEN10 { get; } = (Color)ColorConverter.ConvertFromString("#1F3B00");
        public static Color GREEN20 { get; } = (Color)ColorConverter.ConvertFromString("#315B00");
        public static Color GREEN30 { get; } = (Color)ColorConverter.ConvertFromString("#76B828");
        public static Color GREEN40 { get; } = (Color)ColorConverter.ConvertFromString("#79E600");
        public static Color GREEN50 { get; } = (Color)ColorConverter.ConvertFromString("#99EC3C");
        public static Color GREEN60 { get; } = (Color)ColorConverter.ConvertFromString("#BAF379");
        public static Color GREEN70 { get; } = (Color)ColorConverter.ConvertFromString("#CBF599");
        public static Color GREEN80 { get; } = (Color)ColorConverter.ConvertFromString("#DCF8BB");
        public static Color GREEN90 { get; } = (Color)ColorConverter.ConvertFromString("#EDFBDC");

        public static Color WOOD10 { get; } = (Color)ColorConverter.ConvertFromString("#103518");
        public static Color WOOD20 { get; } = (Color)ColorConverter.ConvertFromString("#185326");
        public static Color WOOD30 { get; } = (Color)ColorConverter.ConvertFromString("#298A40");
        public static Color WOOD40 { get; } = (Color)ColorConverter.ConvertFromString("#48D44C");
        public static Color WOOD50 { get; } = (Color)ColorConverter.ConvertFromString("#71DE75");
        public static Color WOOD60 { get; } = (Color)ColorConverter.ConvertFromString("#9BEA9E");
        public static Color WOOD70 { get; } = (Color)ColorConverter.ConvertFromString("#B3EFB4");
        public static Color WOOD80 { get; } = (Color)ColorConverter.ConvertFromString("#CCF4CD");
        public static Color WOOD90 { get; } = (Color)ColorConverter.ConvertFromString("#E5F9E5");

        public static Color AQUA10 { get; } = (Color)ColorConverter.ConvertFromString("#003B39");
        public static Color AQUA20 { get; } = (Color)ColorConverter.ConvertFromString("#005B58");
        public static Color AQUA30 { get; } = (Color)ColorConverter.ConvertFromString("#009993");
        public static Color AQUA40 { get; } = (Color)ColorConverter.ConvertFromString("#00D4CE");
        public static Color AQUA50 { get; } = (Color)ColorConverter.ConvertFromString("#3ADFDA");
        public static Color AQUA60 { get; } = (Color)ColorConverter.ConvertFromString("#75EAE7");
        public static Color AQUA70 { get; } = (Color)ColorConverter.ConvertFromString("#96EFEC");
        public static Color AQUA80 { get; } = (Color)ColorConverter.ConvertFromString("#B9F4F2");
        public static Color AQUA90 { get; } = (Color)ColorConverter.ConvertFromString("#DBF9F8");

        public static Color CYAN10 { get; } = (Color)ColorConverter.ConvertFromString("#00293B");
        public static Color CYAN20 { get; } = (Color)ColorConverter.ConvertFromString("#00405B");
        public static Color CYAN30 { get; } = (Color)ColorConverter.ConvertFromString("#086B92");
        public static Color CYAN40 { get; } = (Color)ColorConverter.ConvertFromString("#039EE6");
        public static Color CYAN50 { get; } = (Color)ColorConverter.ConvertFromString("#3EB5EC");
        public static Color CYAN60 { get; } = (Color)ColorConverter.ConvertFromString("#7ACDF3");
        public static Color CYAN70 { get; } = (Color)ColorConverter.ConvertFromString("#9BD8F5");
        public static Color CYAN80 { get; } = (Color)ColorConverter.ConvertFromString("#BCE5F8");
        public static Color CYAN90 { get; } = (Color)ColorConverter.ConvertFromString("#DDF1FB");

        public static Color BLUE10 { get; } = (Color)ColorConverter.ConvertFromString("#001D3B");
        public static Color BLUE20 { get; } = (Color)ColorConverter.ConvertFromString("#002D5B");
        public static Color BLUE30 { get; } = (Color)ColorConverter.ConvertFromString("#00559D");
        public static Color BLUE40 { get; } = (Color)ColorConverter.ConvertFromString("#0079E0");
        public static Color BLUE50 { get; } = (Color)ColorConverter.ConvertFromString("#3C98E7");
        public static Color BLUE60 { get; } = (Color)ColorConverter.ConvertFromString("#78B8EF");
        public static Color BLUE70 { get; } = (Color)ColorConverter.ConvertFromString("#99C9F2");
        public static Color BLUE80 { get; } = (Color)ColorConverter.ConvertFromString("#BBDBF6");
        public static Color BLUE90 { get; } = (Color)ColorConverter.ConvertFromString("#DCECFA");

        public static Color VIOLET10 { get; } = (Color)ColorConverter.ConvertFromString("#1D032F");
        public static Color VIOLET20 { get; } = (Color)ColorConverter.ConvertFromString("#2D0549");
        public static Color VIOLET30 { get; } = (Color)ColorConverter.ConvertFromString("#62217E");
        public static Color VIOLET40 { get; } = (Color)ColorConverter.ConvertFromString("#750FB2");
        public static Color VIOLET50 { get; } = (Color)ColorConverter.ConvertFromString("#9447C4");
        public static Color VIOLET60 { get; } = (Color)ColorConverter.ConvertFromString("#B47FD7");
        public static Color VIOLET70 { get; } = (Color)ColorConverter.ConvertFromString("#C69EE0");
        public static Color VIOLET80 { get; } = (Color)ColorConverter.ConvertFromString("#D9BEEA");
        public static Color VIOLET90 { get; } = (Color)ColorConverter.ConvertFromString("#EBDEF4");

        public static Color MAGENTA10 { get; } = (Color)ColorConverter.ConvertFromString("#3A001A");
        public static Color MAGENTA20 { get; } = (Color)ColorConverter.ConvertFromString("#5A0029");
        public static Color MAGENTA30 { get; } = (Color)ColorConverter.ConvertFromString("#A81456");
        public static Color MAGENTA40 { get; } = (Color)ColorConverter.ConvertFromString("#E6026A");
        public static Color MAGENTA50 { get; } = (Color)ColorConverter.ConvertFromString("#EC3E8D");
        public static Color MAGENTA60 { get; } = (Color)ColorConverter.ConvertFromString("#F27AB0");
        public static Color MAGENTA70 { get; } = (Color)ColorConverter.ConvertFromString("#F59BC3");
        public static Color MAGENTA80 { get; } = (Color)ColorConverter.ConvertFromString("#F8BCD6");
        public static Color MAGENTA90 { get; } = (Color)ColorConverter.ConvertFromString("#FBDDEA");
#pragma warning restore CS8605 // Unboxing a possibly null value.

        public static Color PRIMARY_BLUE { get; } = BLUE40;
        public static Color PRIMARY_CYAN { get; } = CYAN40;
        public static Color PRIMARY_GREEN { get; } = MAIGREEN40; 
        public static Color SECONDARY_MAIGREEN { get; } = MAIGREEN30;
        public static Color SECONDARY_GREEN { get; } = GREEN30;
        public static Color SECONDARY_AQUA { get; } = AQUA30;
        public static Color SECONDARY_CYAN { get; } = CYAN30;
        public static Color SECONDARY_BLUE { get; } = BLUE30;
    }
}