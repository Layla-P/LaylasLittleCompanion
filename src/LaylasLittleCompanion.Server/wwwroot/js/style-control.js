var getBodyElement = function () {
	return document.getElementsByTagName("body")[0];
}

export function updateBody() {
	var body = getBodyElement();
	body.style.backgroundColor = "transparent";
	
}


