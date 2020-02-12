using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace etap2.Controllers
{
    [ApiController]
    public class RpnController : ControllerBase
    {
        [HttpGet]
        [Produces("application/json")]
        [Route("api/calculate")]
        public IActionResult Get(string formula, double x)
        {
            Rpn r = new Rpn(formula);
            double result = r.Calculate(x);
            var data = new {
                status="ok",
                result=result
            };
            try {
                using(var db = new HistoryContext())
                {
                    db.Add(new History {
                        formula = formula,
                        x = x,
                        from = null,
                        to = null,
                        n = null
                    });
                    db.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Ok(data);
        }

        [HttpGet]
        [Produces("application/json")]
        [Route("api/calculate/xy")]
        public IActionResult Get(string formula, double from, double to, int n)
        {
            Rpn r = new Rpn(formula);
            List<Point> points = r.CalculateXY(from, to, n);
            try {
                using(var db = new HistoryContext())
                {
                    db.Add(new History {
                        formula = formula,
                        x = null,
                        from = from,
                        to = to,
                        n = n
                    });
                    db.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            var data = new {
                status="ok",
                results=points.ToArray()
            };
            return Ok(data);
        }

        [HttpGet]
        [Produces("application/json")]
        [Route("api/rpn")]
        public IActionResult Get(string formula)
        {
            Rpn r = new Rpn(formula);
            var data = new {
                status="ok",
                results=r.GetTokens().ToArray()
            };
            return Ok(data);
        }

        [HttpGet]
        [Produces("application/json")]
        [Route("api/history")]
        public IActionResult Get(int n=10)
        {
            try {
                using(var db = new HistoryContext())
                {
                    var histories = db.Histories.OrderByDescending(m => m.HistoryId).Take(10).ToList();
                    return Ok(new {
                        status="ok",
                        histories=histories
                    });
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Ok(new {
                status="error"
            });
        }
    }
}
