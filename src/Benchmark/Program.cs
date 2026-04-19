using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;

// THAY ĐỔI URL NÀY THÀNH URL API CỦA BẠN ĐANG CHẠY
var baseUrl = "https://localhost:7153";

// Khởi tạo 1 HttpClient duy nhất để dùng chung
using var httpClient = new HttpClient();

// Kịch bản 1: Test API Good (Dùng Async)
var scenarioGood = Scenario.Create("Test_GetProducts_Good_Async", async context =>
{
    var request = Http.CreateRequest("GET", $"{baseUrl}/api/v1/Products/GetProductsGood");
    var response = await Http.Send(httpClient, request);
    return response;
})
.WithLoadSimulations(
    // Ép hệ thống bắn đúng 1000 requests mỗi giây, duy trì trong 30 giây
    Simulation.Inject(rate: 500, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30))
);

// Kịch bản 2: Test API Bad (Không dùng Async)
var scenarioBad = Scenario.Create("Test_GetProducts_Bad_Sync", async context =>
{
    var request = Http.CreateRequest("GET", $"{baseUrl}/api/v1/Products/GetProductsBad");
    var response = await Http.Send(httpClient, request);
    return response;
})
.WithLoadSimulations(
    // Tương tự: 1000 requests/giây trong 30 giây
    Simulation.Inject(rate: 500, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30))
);

Console.WriteLine("🚀 BẮT ĐẦU CHẠY KỊCH BẢN GOOD (ASYNC)...");
NBomberRunner
    .RegisterScenarios(scenarioGood)
    .WithReportFileName("Good_Async_Report") // Tách riêng file report
    .Run();

Console.WriteLine("⏳ Đang nghỉ 5 giây để Windows dọn dẹp Socket và Thread Pool nhả luồng...");
await Task.Delay(5000); // Rất quan trọng: cho server "thở" một nhịp

Console.WriteLine("🚀 BẮT ĐẦU CHẠY KỊCH BẢN BAD (SYNC)...");
NBomberRunner
    .RegisterScenarios(scenarioBad)
    .WithReportFileName("Bad_Sync_Report") // Tách riêng file report
    .Run();

Console.WriteLine("✅ ĐÃ XONG CẢ 2 KỊCH BẢN!");
Console.ReadLine();
