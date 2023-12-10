const cartItemHolder = document.querySelector(".cart-item-holder");
const increment = document.querySelector(".increment");
const basketCount = document.querySelector(".basket-count");
const removeFromCart = document.querySelector(".remove-from-cart");
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
                        increment.forEach(holder => {
                            const currentCount = parseInt(holder.textContent.trim());
                            if (!isNaN(currentCount)) {
                                holder.textContent = currentCount + 1;
                                basketCount.innerHTML = currentCount;
                            }
                        });
                        removeFromCart.forEach(removeButton => {
                            removeButton.addEventListener("click", ev => {
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
                                                updateCountHolders(); 
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
