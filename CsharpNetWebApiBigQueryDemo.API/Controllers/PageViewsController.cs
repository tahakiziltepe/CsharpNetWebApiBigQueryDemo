using CsharpNetWebApiBigQueryDemo.API.Models;
using Google.Cloud.BigQuery.V2;
using Microsoft.AspNetCore.Mvc;

namespace CsharpNetWebApiBigQueryDemo.API.Controllers
{
    [ApiController]
    [Route("wikiapi")]
    public class PageViewsController : ControllerBase
    {
        private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT");
        private readonly BigQueryClient _bigQueryClient;

        public PageViewsController(ILogger<PageViewsController> logger)
        {
            if (string.IsNullOrEmpty(_projectId))
                throw new Exception("Default project has not been set. Please set the GOOGLE_CLOUD_PROJECT environment variable.");
            else
                _bigQueryClient = BigQueryClient.Create(_projectId);
        }


        [HttpGet("TopViewsYesterday")]
        public async Task<IActionResult> GetPageViews()
        {
            var query = @"
                SELECT DATE(datehour) AS `date`, title, wiki, SUM(views) AS sum_views
                FROM `bigquery-public-data.wikipedia.pageviews_2025`
                WHERE date(datehour) = date_sub(current_date(), interval 1 day)
                  AND contains_substr(wiki, 'tr') and lower(title) != 'anasayfa'
                GROUP BY ALL
                ORDER BY SUM(views) DESC
                LIMIT 10
                ";
            try
            {
                //var results = await _bigQueryClient.ExecuteQueryAsync(query, parameters: null);            
                //var list = results
                //        .Select(row => new PageViews
                //        {
                //            Title = row["title"].ToString(),
                //            Wiki = row["wiki"].ToString(),
                //            Views = Convert.ToInt32(row["views"]),
                //            DateHour = (DateTime)row["datehour"]
                //        })
                //        .ToList();

                BigQueryJob job = await _bigQueryClient.CreateQueryJobAsync(query, parameters: null);
                await job.PollUntilCompletedAsync();
                BigQueryResults results = await _bigQueryClient.GetQueryResultsAsync(job.Reference);

                if (results == null)
                {
                    return NotFound(new BaseResponseModel { Success = false, ErrorMessage = "No data found" });
                }

                var pageViewsList = new List<PageViews>();
                await foreach (var row in results.GetRowsAsync())
                {
                    pageViewsList.Add(new PageViews
                    {
                        Title = row["title"].ToString(),
                        Wiki = row["wiki"].ToString(),
                        Views = Convert.ToInt32(row["sum_views"]),
                        DateHour = (DateTime)row["date"]
                    });
                }
                return Ok(new BaseResponseModel { Success = true, Data = pageViewsList });

            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
    }
}
