using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using hr_client.Models;
using hr_client.Services;
using Moq;
using Moq.Protected;
using Xunit;

namespace hr_client_tests
{
    public class EmployeesServiceTests
    {
        [Fact]
        public async void GetAll_ShouldFetchEmployeesAndReturnsThem()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("[{\"employeeId\": \"someId\" }]"),
               })
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://localhost:5001/"),
            };

            var service = new EmployeesService(httpClient);

            var result = await service.GetAll();

            var expected = new List<EmployeeViewModel>
            {
                new EmployeeViewModel
                {
                    employeeId = "someId"
                }
            };
    
            Assert.Equal(expected.First().employeeId, result.First().employeeId);
        }

        [Fact]
        public async void GetByStatus_ShouldBuildValidUrl()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("[{\"employeeId\": \"someId\", \"status\": true }]"),
               })
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://localhost:5001/"),
            };

            var service = new EmployeesService(httpClient);

            await service.GetByStatus("active");

            var expectedUri = new Uri("https://localhost:5001/employees?status=active");
            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
                  && req.RequestUri == expectedUri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async void AddNewEmployee_ShouldCallEndpoint()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.Created,
               })
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://localhost:5001/"),
            };

            var service = new EmployeesService(httpClient);

            var newEmployee = new EmployeeViewModel();

            await service.AddNewEmployee(newEmployee);

            var expectedUri = new Uri("https://localhost:5001/employees");
            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Put
                  && req.RequestUri == expectedUri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async void ChangeStatus_ShouldCallEndpoint()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.Created,
               })
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://localhost:5001/"),
            };

            var service = new EmployeesService(httpClient);

            var newEmployee = new EmployeeViewModel();

            await service.ChangeStatus("some_id", "active");

            var expectedUri = new Uri("https://localhost:5001/employees/some_id");
            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post
                  && req.RequestUri == expectedUri
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
