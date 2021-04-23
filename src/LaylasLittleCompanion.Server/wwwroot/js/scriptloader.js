export async function loadScript(scriptPath) {
	var script = document.createElement("script");
	script.src = scriptPath;
	script.type = "text/javascript";
	console.log(scriptPath + " created");

	// flag as loading/loaded
	//loaded[scriptPath] = true;

	// if the script returns okay, return resolve
	script.onload = function () {
		console.log(scriptPath + " loaded ok");
		//resolve(scriptPath);
	};

	// if it fails, return reject
	script.onerror = function () {
		console.log(scriptPath + " load failed");
		//reject(scriptPath);
	}

	// scripts will load at end of body
	document["body"].appendChild(script);
}
