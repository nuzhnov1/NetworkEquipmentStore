function onReadImage() {
    let input = document.getElementById("MainContent_ProductImageFile");

    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            let image = document.getElementById("product-image");
            image.src = e.target.result;
        };

        reader.readAsDataURL(input.files[0]);
    }
}
