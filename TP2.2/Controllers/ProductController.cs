﻿using Microsoft.AspNetCore.Mvc;


using TP2._2.Models.Repositeries;
using TP2._2.Models;
using TP2._2.ViewModels;

namespace TP2._2.Controllers
{
    public class ProductController : Controller
    {
        readonly IRepository<Product> productRepository;
        private readonly IWebHostEnvironment hostingEnvironment;


     
        public ProductController(IRepository<Product> ProdRepository, IWebHostEnvironment hostingEnvironment)
        {
            productRepository = ProdRepository;
            this.hostingEnvironment = hostingEnvironment;
        }


        public ActionResult Index()
        {
            var Produits = productRepository.GetAll();
            return View(Produits);
        }

        // GET: ProductController/Details/5
        public ActionResult Details(int id)
        {

            return View(productRepository.Get(id));
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.ImagePath != null)
                {
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    model.ImagePath.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                Product newProduct = new Product
                {
                    Désignation = model.Désignation,
                    Prix = model.Prix,
                    Quantite = model.Quantite,
                    // Store the file name in PhotoPath property of the employee object
                    // which gets saved to the Employees database table
                    Image = uniqueFileName
                };
                productRepository.Add(newProduct);
                return RedirectToAction("details", new { id = newProduct.Id });
            }
            return View();

        }

        // GET: ProductController/Edit/5
        // GET: ProductController/Edit/5
        public ActionResult Edit(int id)
        {
            Product product = productRepository.Get(id);
            EditViewModel productEditViewModel = new EditViewModel
            {
                Id = product.Id,
                Désignation = product.Désignation,
                Prix = product.Prix,
                Quantite = product.Quantite,
                ExistingImagePath = product.Image
            };
            return View(productEditViewModel);
        }
        public class EditViewModel : CreateViewModel
        {
            public int Id { get; set; }
            public string ExistingImagePath { get; set; }
        }
        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditViewModel model)
        {
            // Check if the provided data is valid, if not rerender the edit view
            // so the user can correct and resubmit the edit form
            if (ModelState.IsValid)
            {
                // Retrieve the product being edited from the database
                Product product = productRepository.Get(model.Id);
                // Update the product object with the data in the model object
                product.Désignation = model.Désignation;
                product.Prix = model.Prix;
                product.Quantite = model.Quantite;
                // If the user wants to change the photo, a new photo will be
                // uploaded and the Photo property on the model object receives
                // the uploaded photo. If the Photo property is null, user did
                // not upload a new photo and keeps his existing photo
                if (model.ImagePath != null)
                {
                    // If a new photo is uploaded, the existing photo must be
                    // deleted. So check if there is an existing photo and delete
                    if (model.ExistingImagePath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingImagePath);
                        System.IO.File.Delete(filePath);
                    }
                    // Save the new photo in wwwroot/images folder and update
                    // PhotoPath property of the product object which will be
                    // eventually saved in the database
                    product.Image = ProcessUploadedFile(model);
                }
                // Call update method on the repository service passing it the
                // product object to update the data in the database table
                Product updatedProduct = productRepository.Update(product);
                if (updatedProduct != null)
                    return RedirectToAction("Index");
                else
                    return NotFound();
            }
            return View(model);
        }
        [NonAction]
        private string ProcessUploadedFile(EditViewModel model)
        {
            string uniqueFileName = null;
            if (model.ImagePath != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImagePath.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }


        // GET: ProductController/Delete/5
        public ActionResult Delete(int id)
        {

            return View(productRepository.Get(id));
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Product p)
        {
            try
            {
                productRepository.Delete(p.Id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }

}