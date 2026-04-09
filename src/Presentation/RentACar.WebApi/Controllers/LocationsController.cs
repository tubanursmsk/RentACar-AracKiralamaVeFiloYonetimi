using Microsoft.AspNetCore.Mvc;
using RentACar.Application.DTOs.Responses;
using RentACar.Domain.Entities;
using RentACar.Domain.Interfaces;

namespace RentACar.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public LocationsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var locations = await _unitOfWork.Locations.GetAllAsync();
            return Ok(ApiResponse<object>.SuccessResult(locations));
        }

        public class LocationInputModel { public string Name { get; set; } = string.Empty; public string City { get; set; } = string.Empty; }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] LocationInputModel model)
        {
            var location = new Location { Name = model.Name, City = model.City };
            await _unitOfWork.Locations.AddAsync(location);
            await _unitOfWork.SaveChangesAsync();
            return Ok(ApiResponse<object>.SuccessResult(location, "Şube eklendi."));
        }
    }
}


/*

Servisleri Yazmadan Kayıt Nasıl Başarılı Oldu?
Bu durum tamamen Entity Framework Core'un (EF Core) yeteneklerinden kaynaklanıyor. Swagger üzerinden veri
gönderdiğinde arka planda şunlardan biri oldu:
Sadece ID Gönderdiysen: Eğer Swagger üzerinden JSON payload'unda 
sadece BrandId: 1, CurrentLocationId: 2 
gibi değerler gönderdiysen ve veri tabanında (veya bellekte) yabancı anahtar (Foreign Key) kısıtlaması
henüz katı bir şekilde patlamadıysa, EF Core sadece o int değerleri Cars tablosuna yazar. Marka veya 
lokasyonun adının ne olduğuyla o an ilgilenmez, sadece ilişkiyi kurar.

İç İçe Nesne (Nested Object) Gönderdiysen (Deep Insert): Eğer JSON içinde Brand: { Name: "Toyota" }
gibi tüm nesneyi gönderdiysen, EF Core'un "Object Graph" (Nesne Grafiği) takibi devreye girer. EF Core
bakar ki sen bir Araba (Car) ekliyorsun, ama içinde daha önce veri tabanında olmayan bir Marka
(Brand) da var. EF Core senin yerine önce gider o markayı Brands tablosuna kaydeder, oluşan ID'yi alır,
sonra o ID ile Arabayı Cars tablosuna kaydeder.

Yani EF Core, doğrudan DbContext üzerinden tabloları yönetebildiği için senin ayrıca bir BrandServiceçağırmana gerek kalmadan işlemi arka planda halleder.

*/