"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();


connection.on("LaylaMessage", function (user, message, action) {
   
    //if (action === "cannon") {
    //    triggerCannon();
    //}
    //else {
        let count = action === "super" ? 50 : 13;
        let image = action === "waffle" ? "images/waffle.png" : "destructopup-112.png";
        triggerRain(count, image);
    //}
   
    
});

connection.start().then(function () {
   
}).catch(function (err) {
    return console.error(err.toString());
});

const world = document.querySelector(".boops");
const { Engine, Render, Runner, World, Bodies } = Matter;

function createBall(image) {
    const ball = Bodies.circle(Math.round(Math.random() * 1280), -30, 25, {
        angle: Math.PI * (Math.random() * 2 - 1),
        friction: 0.001,
        frictionAir: 0.01,
        restitution: 0.8,
        render: {
            sprite: {
                texture: image                
            }
        }
    });

    setTimeout(() => {
        World.remove(engine.world, ball);
    }, 30000);

    return ball;
}

const engine = Engine.create();
const runner = Runner.create();
const render = Render.create({
    canvas: world,
    engine: engine,
    options: {
        width: 1280,
        height: 720,
        background: "transparent",
        wireframes: false
    }
});

const boundaryOptions = {
    isStatic: true,
    render: {
        fillStyle: "transparent",
        strokeStyle: "transparent"
    }
};
const ground = Bodies.rectangle(640, 720, 1300, 4, boundaryOptions);
const leftWall = Bodies.rectangle(0, 360, 4, 740, boundaryOptions);
const rightWall = Bodies.rectangle(1280, 360, 4, 800, boundaryOptions);
Render.run(render);
Runner.run(runner, engine);

World.add(engine.world, [ground, leftWall, rightWall]);


const triggerRain = (count, image) => {

    for (let i = 0; i < count; i++) {
        World.add(engine.world, [createBall(image)])
    }
};


///confetti code

const box = $('#confetti');
const className = ".confetti";
const elementType = "<div></div>";
const minHeight = 5;
const maxHeight = 15;
const minWidth = 20;
const maxWidth = 30;
const startPosTop = 0;
const startPosLeft = 0;
const floor = 100;
const minFireLeft = 500;
const maxFireLeft = 1200;
const minAnimDuration = 7;
const maxAnimDuration = 10;
const confettiAmount = 200;
const colorArr = ['#FF6633', '#FFB399', '#FF33FF', '#FFFF99', '#00B3E6', '#E6B333', '#3366E6', '#999966', '#99FF99', '#B34D4D', '#80B300', '#809900', '#E6B3B3', '#6680B3', '#66991A', '#FF99E6', '#CCFF1A', '#FF1A66', '#E6331A', '#33FFCC', '#66994D', '#B366CC', '#4D8000', '#B33300', '#CC80CC', '#66664D', '#991AFF', '#E666FF', '#4DB3FF', '#1AB399', '#E666B3', '#33991A', '#CC9999', '#B3B31A', '#00E680', '#4D8066', '#809980', '#E6FF80', '#1AFF33', '#999933', '#FF3380', '#CCCC00', '#66E64D', '#4D80CC', '#9900B3', '#E64D66', '#4DB380', '#FF4D4D', '#99E6E6', '#6666FF'];

let readyToFire = true;
function CreateConfetti(amount) {

    for (let i = 0; i < amount; i++) {

        // Giving the confetti a random height and width
        const h = Rand(minHeight, maxHeight);
        const w = Rand(minWidth, maxWidth);

        // Generating the confetti background color
        const color = colorArr[Rand(0, colorArr.length)];

        // Creating the confetti as the specified element type
        const c = $(elementType);

        // Setting height and width
        c.height(h);
        c.width(w);

        // Adding class with global styling
        c.addClass("confetti");

        // Setting the position and background color of the confetti
        c.css({
            "background-color": color,
            "left": startPosLeft,
            "top": startPosTop
        });

        // Inserts the confetti into the container
        box.append(c);
    }

}

function Fire() {

    // Check to see if cannon is ready to fire
    if (readyToFire) {

        readyToFire = false;

        // Loops through all confetti inside the container
        box.children(className).each(function () {

            const c = $(this);

            // Duration of the transitions
            const dur = RandFloat(minAnimDuration, maxAnimDuration);

            // X and Y rotation in degrees
            const xRotation = Rand(1440, 3600);
            const zRotation = Rand(540, 1440);

            // Controls the vertical spread of the confetti, the bigger the difference between the two numbers, the larger the spread
            const verticalSpread = RandFloat(4, 8);

            // Controls how clumped the confetti falls
            // Second value cannot be above 1
            // This is used to prevent them all hitting the ground at the same time
            // The bigger the difference between the two numbers, the larger the spread
            const fallingClump = RandFloat(-5, 1);

            // Delay before transition (animation) starts
            // This is used to prevent all the confetti being fired in a clump
            const delay = RandFloat(0, 1);

            // Setting up the three different transition properties for the element
            const cTransitionTop = "top " + dur + "s cubic-bezier(0, -" + verticalSpread + ", 0.1, " + fallingClump + ") " + delay + "s";
            const cTransitionLeft = "left " + dur + "s cubic-bezier(0, 1, 0.1, 1) " + delay + "s";
            const cTransitionTransform = "transform " + dur + "s linear " + delay + "s";

            // Stitching the transitions together
            const cTransition = cTransitionTop + ", " + cTransitionLeft + ", " + cTransitionTransform;

            // Left property for the element
            const cLeft = Rand(minFireLeft, maxFireLeft);

            // Top property for the element
            const cTop = floor + "%";

            // Transform (i.e. the rotational movement) for the element
            const cTransform = "rotateX(" + xRotation + "deg) rotateZ(" + zRotation + "deg)";

            // Adding the CSS properties to the confetti
            c.css({
                "transition": cTransition,
                "left": cLeft,
                "top": cTop,
                "transform": cTransform
            });

        });

    }

}

// Resets the position of each confetti
function ResetConfetti() {
    box.children(className).each(function () {

        const c = $(this);

        c.css({
            "transition": "",
            "top": startPosTop,
            "left": startPosLeft,
            "transform": ""
        });

    });

    readyToFire = true;
}

// Returns random whole number
function Rand(min, max) {
    return Math.floor(Math.random() * (max - min + 1) + min);
}

// Returns random float with two decimals
function RandFloat(min, max) {
    let a = Math.random() * (max - min) + min;

    a = a.toFixed(2);

    return a;
}

CreateConfetti(confettiAmount);


const triggerCannon = () => {

    Fire();
    setTimeout(() => {
        ResetConfetti();
    }, 30000);
};



