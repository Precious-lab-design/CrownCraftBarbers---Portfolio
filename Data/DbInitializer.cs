using CrownCraftBarbers.Models;

namespace CrownCraftBarbers.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Services.Any())
            {
                context.Services.AddRange(
                    new Service
                    {
                        Name = "Signature Haircut",
                        Description = "A classic precision haircut finished with a clean styling finish.",
                        Price = 180,
                        DurationMinutes = 45,
                        Icon = "✂"
                    },

                    new Service
                    {
                        Name = "Skin Fade",
                        Description = "A sharp skin fade blended to your preferred style.",
                        Price = 220,
                        DurationMinutes = 60,
                        Icon = "✦"
                    },

                    new Service
                    {
                        Name = "Beard Trim",
                        Description = "Professional beard shaping, trimming and finishing.",
                        Price = 120,
                        DurationMinutes = 30,
                        Icon = "◈"
                    },

                    new Service
                    {
                        Name = "Haircut + Beard",
                        Description = "A complete haircut and beard grooming experience.",
                        Price = 280,
                        DurationMinutes = 75,
                        Icon = "♔"
                    },

                    new Service
                    {
                        Name = "Kids Cut",
                        Description = "A stylish, comfortable haircut for younger clients.",
                        Price = 130,
                        DurationMinutes = 40,
                        Icon = "★"
                    },

                    new Service
                    {
                        Name = "The Royal Package",
                        Description = "Haircut, beard grooming and premium finishing treatment.",
                        Price = 350,
                        DurationMinutes = 90,
                        Icon = "♛"
                    }
                );

                context.SaveChanges();
            }

            if (!context.Barbers.Any())
            {
                context.Barbers.AddRange(
                    new Barber
                    {
                        Name = "Marcus",
                        Role = "Master Barber",
                        Bio = "Known for precision cuts, classic styles and detailed finishing.",
                        ImageUrl = "/images/Senior Barber.jpg"
                    },

                    new Barber
                    {
                        Name = "Thabo",
                        Role = "Senior Barber",
                        Bio = "Specialises in modern fades, beard grooming and sharp finishes.",
                        ImageUrl = "/images/Master barber.jpg"
                    },

                    new Barber
                    {
                        Name = "Liam",
                        Role = "Barber",
                        Bio = "Creates modern cuts with a relaxed and welcoming approach.",
                        ImageUrl = "/images/Barber.jpg"
                    }
                );

                context.SaveChanges();
            }
        }
    }
}