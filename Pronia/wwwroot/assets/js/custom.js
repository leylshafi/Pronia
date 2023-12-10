const cartItemHolder = document.querySelector(".cart-item-holder");
const addToCartButtons = document.querySelectorAll(".add-to-cart");
const countHolders = document.querySelectorAll(".count-holder");

addToCartButtons.forEach(button => {
    button.addEventListener("click", ev => {
        ev.preventDefault();
        const closestAnchor = ev.target.closest("a");
        if (closestAnchor) {
            const href = closestAnchor.getAttribute("href");

            if (href) {
                fetch(href)
                    .then(res => res.text())
                    .then(data => {
                        console.log(data);
                        cartItemHolder.innerHTML = data;

                        countHolders.forEach(holder => {
                            const currentCount = parseInt(holder.textContent.trim());
                            if (!isNaN(currentCount)) {
                                holder.textContent = currentCount + 1;
                            }
                        });
                    })
                    .catch(error => {
                        console.error('Fetch error:', error);
                    });
            } else {
                console.error('Href attribute not found');
            }
        }
    });
});