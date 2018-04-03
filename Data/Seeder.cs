using System.Linq;

namespace LinealCutOptimizer.Core.Data
{
    public static class Seeder
    {
        public static void Seed()
        {
            using (var context = new EFLinearCutSolutionEntities())
            {
                SeedMeasurements(context);
                SeedParams(context);
                SeedBarPatterns(context);


                context.SaveChanges();
            }
        }

        private static void SeedBarPatterns(EFLinearCutSolutionEntities context)
        {
            if (!context.BarPatterns.Any())
            {
                var pattern = new BarPattern
                {
                    Diameter = "10mm",
                    Length = 9,
                    MinLengthReusable = 0.7M
                };
                context.BarPatterns.Add(pattern);
            }
        }

        private static void SeedParams(EFLinearCutSolutionEntities context)
        {
            if (!context.Params.Any())
            {
                var param = new Params
                {
                    MeasurementUnit = context.MeasurementUnit.FirstOrDefault(m => m.Code == "m")
                };
                context.Params.Add(param);
            }
        }

        private static void SeedMeasurements(EFLinearCutSolutionEntities context)
        {
            if (!context.MeasurementUnit.Any(m => m.Code == "mm"))
            {
                var um = new MeasurementUnit
                {
                    Code = "mm",
                    Name = "Milímetros",
                };
                context.MeasurementUnit.Add(um);
            }
            if (!context.MeasurementUnit.Any(m => m.Code == "cm"))
            {
                var um = new MeasurementUnit
                {
                    Code = "cm",
                    Name = "Centímetros",
                };
                context.MeasurementUnit.Add(um);
            }
            if (!context.MeasurementUnit.Any(m => m.Code == "inch"))
            {
                var um = new MeasurementUnit
                {
                    Code = "inch",
                    Name = "Pulgadas",
                };
                context.MeasurementUnit.Add(um);
            }
            if (!context.MeasurementUnit.Any(m => m.Code == "m"))
            {
                var um = new MeasurementUnit
                {
                    Code = "m",
                    Name = "Metros",
                };
                context.MeasurementUnit.Add(um);
            }
            context.SaveChanges();
        }
    }
}