const bar = document.getElementById("Bar");
let count = document.getElementById("Count").value;
console.log(count);
let width = Math.floor((count / 300) * 100)


if (width > 100) {
    width = 100;
}
if (width < 15) {
    document.getElementById("Bar-info").classList.add("info-above");
}


let barPercent = 100 - width;
bar.style.transform = 'translateY(' + barPercent + '%)'
