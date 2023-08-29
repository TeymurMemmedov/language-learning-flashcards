//Local storage-ə 
if (localStorage.getItem("userPreferences") == null) {
    localStorage.setItem("userPreferences", JSON.stringify(
        {
            speed: "normal",
            frontOfCard: "terms",
            orderOfCards: "default"
        }
    ))
}

console.log("salam")

let inputs = document.querySelectorAll("input");
if (inputs != null) {
    inputs.forEach(inputElement => 
        inputElement.addEventListener("input",
            () => {
                inputElement.classList.remove("input-validation-error")
            }
        )
    )
}

let hideOpenBtn = document.getElementById("hide-open-button");
let passwordInput = document.getElementsByName("Password")[0]
let rePasswordInput = document.getElementsByName("RePassword")[0]
console.log(passwordInput)
let isHideOpenClicked = false
if (hideOpenBtn != null) {
    hideOpenBtn.addEventListener("click", function () {

        if (isHideOpenClicked) {
            passwordInput.setAttribute("type", "password");
            if (rePasswordInput!=null){
            rePasswordInput.setAttribute("type", "password");
            }
            hideOpenBtn.querySelector("i").classList.replace("fa-eye-slash", "fa-eye");
        }
        else {
    passwordInput.setAttribute("type", "text");
    if (rePasswordInput!=null) {
        rePasswordInput.setAttribute("type", "text");
    }
            hideOpenBtn.querySelector("i").classList.replace("fa-eye", "fa-eye-slash");
        }
        isHideOpenClicked = !isHideOpenClicked
       
        
    }
    )
}




const termContainer = document.querySelector("#creating-term-container");
const addMoreItem = document.getElementById("addMoreTerms");

function createTermInputGroup(termCount) {
    return `
        <div class="form-group d-flex align-items-center justify-content-between mb-3">
            <div class="col-4">
                <label class="form-label termLabel">Term ${termCount + 1}</label>
                <input type="text"  class="form-control term custom-form-input" name="TermsViewModels[${termCount}].Term" />
                  <span class="text-white" asp-validation-for="TermsViewModels[${termCount}].Term"></span>
            </div>
            <div class="col-5">
                <label class="form-label">Definiton</label>
                <textarea rows="1" type="text" class="form-control autoresizing def custom-form-input" name="TermsViewModels[${termCount}].TermDefinition"></textarea>
            </div>
            <div class="col-2 pt-4">
                <button class="btn bg-white delete-term-button ms-2" type="button">
                    <i class=" btn bg-white fa-sharp fa-solid fa-trash text-danger"></i>
                </button>
            </div>
        </div>`;
}


const textarea = document.querySelectorAll(".autoresizing");
if (textarea != null) {
    textarea.forEach(i => i.addEventListener("input", autoResize));
}

function renderTermInputGroup(termCount) {
    const newTermInputGroup = document.createElement('div');
    newTermInputGroup.innerHTML = createTermInputGroup(termCount);
    termContainer.appendChild(newTermInputGroup);

    const textarea = newTermInputGroup.querySelectorAll(".autoresizing");
    textarea.forEach(i => i.addEventListener("input", autoResize));

    const deleteButton = newTermInputGroup.querySelector(".delete-term-button");
    deleteButton.addEventListener("click", () => {
        newTermInputGroup.remove();
        changeOrder();
    });
    console.log("salam")
}

function changeOrder() {
    const termInputs = termContainer.querySelectorAll(".term");
    const defInputs = termContainer.querySelectorAll(".def");
    const termLabels = termContainer.querySelectorAll(".termLabel");

    termInputs.forEach((termInput, index) => {
        const termCount = index + 1;
        termLabels[index].innerHTML = `İfadə ${termCount}`;
        termInput.name = `TermsViewModels[${index}].Term`;
        defInputs[index].name = `TermsViewModels[${index}].TermDefinition`;
    });
}

function autoResize() {
    this.removeAttribute("rows");
    this.style.height = 'auto';
    this.style.height = this.scrollHeight + 'px';
}
if (addMoreItem != null) {
    addMoreItem.addEventListener('click', () => {
        const termCount = termContainer.children.length;
        renderTermInputGroup(termCount);
        array.push("salam");
    });
}

let deleteTermButtons = document.querySelectorAll(".delete-term-button")
deleteTermButtons.forEach(x => x.classList.add("disabled"))



let modalOpeners = document.getElementsByClassName("toggleMyModal");
modalOpeners = Array.from(modalOpeners);


modalOpeners.forEach(x => x.addEventListener('click', function () {
    let card = x.parentElement.parentElement.parentElement;
    let cardTitle = card.querySelector(".card-title").innerHTML;
    var myModal2 = document.getElementById('staticBackdrop');
    myModal2.innerHTML = `
    <div class="modal-dialog modal-dialog-centered">
    <div class="modal-content">
      <div class="modal-header">
        <h2 class="modal-title fs-5" id="staticBackdropLabel">Are you sure you want to delete the set <br> "${cardTitle}" ?</h2>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body">
        "The set and cards will be permanently deleted..
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
        <a href="DeleteSet/${card.id}" type="button" class=" btn btn-danger">Delete</a>
      </div>
    </div>
  </div>
  `;
    var myModal = new bootstrap.Modal(myModal2);
    myModal.show();
}));

let cards = document.querySelector(".allTermSetContainer")
if (cards != null) {
    cards = cards.getElementsByClassName("card-body");
    cards = Array.from(cards)
    cards.forEach(x => x.addEventListener('click', function () {
        localStorage.getItem("userPreferences").orderOfCards
        window.location.href = `LearnSet/${x.parentElement.id}?orderOfCards=${JSON.parse(localStorage.getItem("userPreferences")).orderOfCards}`
    }))
}




/*LearnSet js*/

let myCarousel = document.querySelector('#carouselExampleInterval');
let slideNumber = document.querySelector(".slider-number span");
if (myCarousel !== null) {
    slideNumber.innerHTML = `${findActiveCardOrder()} / ${myCarousel.firstElementChild.childElementCount}`
}


function findActiveCardOrder() {
    let cards = document.querySelectorAll(".carousel-item");
    cards = Array.from(cards)
    let index = cards.findIndex(card => card.className.includes("active"))
    return index + 1;
}


let sliderOptions = {
    speedOptions:
    {
        interval: 3000,
        speeds:
        {
            slow: 0.75,
            normal: 1,
            fast: 1.75
        }
    },
    changeTheFaces() {
        let cardFronts = document.getElementsByClassName("card-front");
        let cardBacks = document.getElementsByClassName("card-back");
        cardFronts = Array.from(cardFronts)
        cardBacks = Array.from(cardBacks)
        for (let i = 0; i < cardFronts.length; i++) {
            temp = cardFronts[i].className
            cardFronts[i].className = cardBacks[i].className
            cardBacks[i].className = temp
        }
    }
}
// Sayti ilk defe acanlarinda brauzerinde bele bir sey yoxdu :()


let userPreferences = JSON.parse(localStorage.getItem("userPreferences"))
let settingsModal = document.querySelector("#settingsModal")
if (settingsModal != null) {
let allInputs = settingsModal.querySelectorAll("input[type='radio']")

    allInputs.forEach(input => {
        if (input.value == userPreferences.speed || input.value == userPreferences.frontOfCard || input.value == userPreferences.orderOfCards) {
            input.checked = true
        }
    })
}
// carousel["_config"].interval = (sliderOptions. speedOptions.interval/sliderOptions.speedOptions.speeds[userPreferences.speed])*4
carousel = new bootstrap.Carousel(myCarousel,
    {
        interval: (sliderOptions.speedOptions.interval / sliderOptions.speedOptions.speeds[userPreferences.speed]) * 2 + 600 * sliderOptions.speedOptions.speeds[userPreferences.speed],
        ride: false,
        touch: true,
        wrap: false,
        pause: false
    })
if (userPreferences.frontOfCard == "definitions") {
    sliderOptions.changeTheFaces()
}
let settingSaveBtn = document.getElementById("saveSettings");
if (settingSaveBtn != null) {
    settingSaveBtn.addEventListener("click", function () {
        let speedPreference = document.querySelector("input[type='radio'][name=speed]:checked").value;
        let frontOfCardPreference = document.querySelector("input[type='radio'][name=frontOfCard]:checked").value;
        let orderOfCardsPreference = document.querySelector("input[type='radio'][name=orderOfCards]:checked").value;
        localStorage.removeItem("userPreferences")
        localStorage.setItem("userPreferences", JSON.stringify(
            {
                speed: speedPreference,
                frontOfCard: frontOfCardPreference,
                orderOfCards: orderOfCardsPreference
            }
        ))

        let settingsModal = document.querySelector("#settingsModal")
        let allInputs = settingsModal.querySelectorAll("input[type='radio']")
        allInputs.forEach(input => input.checked = false)
        // for (let i = 0; i < allInputs.length; i++) {
        //     allInputs[i].checked = false
        // }
        userPreferences = JSON.parse(localStorage.getItem("userPreferences"))
        allInputs.forEach(input => {
            if (input.value == userPreferences.speed || input.value == userPreferences.frontOfCard || input.value == userPreferences.orderOfCards) {
                input.checked = true
            }
        })
        userPreferences = JSON.parse(localStorage.getItem("userPreferences"))
        var url = new URL(location.href)

        url.searchParams.set('orderOfCards', userPreferences.orderOfCards); // Replace 'new_value' with the desired value

        var modifiedUrl = url.toString();

        window.location.href = modifiedUrl

        /*location.reload();*/


    })
}


//Sliderin irəli geri idarə edilməsi
let nextSlide = document.querySelector("#next")
let prevSlide = document.querySelector("#prev")

if (nextSlide != null && prevSlide != null) {
    nextSlide.addEventListener("click",
        function () {
            carousel.next()
        })

    prevSlide.addEventListener("click",
        function () {
            carousel.prev()
        })
}


isPauseBtnClicked = false
let pauseBtn = document.getElementById("pause");
if (pauseBtn != null) {
    let pauseBtnIcon = document.querySelector("#pause i");
    pauseBtn.addEventListener("click", function () {
        isPauseBtnClicked = !isPauseBtnClicked
        if (isPauseBtnClicked) {
            carousel.pause()
            pauseBtnIcon.className = "fa-solid fa-play"
        }
        else {
            carousel.cycle()
            pauseBtnIcon.className = "fa-solid fa-pause"
        }

    })
}



//Kartları çevirmək
function flipCard(card) {
    if (card.style.transform == "" || card.style.transform == "rotateX(0deg)") {
        card.style.transform = "rotateX(180deg)"
    }

    else {
        card.style.transform = "rotateX(0deg)"
    }


}

let cardInners = document.getElementsByClassName("card-inner");
cardInners = Array.from(cardInners);
cardInners.forEach(card => card.addEventListener("click",
    function () {
        flipCard(card)
    }
))



//auto play
function flipCardWithPause(card, pauseAmountWithMillisecond) {
    setTimeout(function () {
        flipCard(card)
    }, pauseAmountWithMillisecond)
}
let isAutoPlayerClicked = false
let autoPlayer = document.querySelector("#autoplay")
if (autoPlayer != null) {
    autoPlayer.addEventListener("click", function () {
        isAutoPlayerClicked = !isAutoPlayerClicked
        let card = document.getElementById("carouselExampleInterval").getElementsByClassName("active")[0].firstElementChild;
        flipCardWithPause(card, sliderOptions.speedOptions.interval / sliderOptions.speedOptions.speeds[userPreferences.speed])

        myCarousel.addEventListener('slid.bs.carousel', function () {
            if (!isPauseBtnClicked) {
                let card = document.getElementById("carouselExampleInterval").getElementsByClassName("active")[0].firstElementChild;
                flipCardWithPause(card, sliderOptions.speedOptions.interval / sliderOptions.speedOptions.speeds[userPreferences.speed])
                setTimeout(() => {
                    if (findActiveCardOrder() == myCarousel.firstElementChild.childElementCount && isAutoPlayerClicked) {
                        setTimeout(
                            () => {
                                alert("You have finished the set. It will restart again. Keep learning!")
                                location.reload();
                            },

                        )


                    }
                },13000)
              
            }
          

        })
        myCarousel.addEventListener('slide.bs.carousel', function () {
            let card = document.getElementById("carouselExampleInterval").getElementsByClassName("active")[0].firstElementChild;
            flipCardWithPause(card, 425)


        })
    
        carousel.cycle();
     
        setTimeout(() => { autoPlayer.style.display = "none" }, 500)




    })
}

if (myCarousel != null) {
    myCarousel.addEventListener("slid.bs.carousel", function () {
        slideNumber.innerHTML = `${findActiveCardOrder()} / ${myCarousel.firstElementChild.childElementCount}`
    
    })
}




