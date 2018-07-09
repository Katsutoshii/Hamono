let OSName = 'Unknown OS';
if (navigator.appVersion.indexOf('Win') != -1) OSName = 'Windows'
if (navigator.appVersion.indexOf('Mac') != -1) OSName = 'MacOS'
if (navigator.appVersion.indexOf('X11') != -1) OSName = 'UNIX'
if (navigator.appVersion.indexOf('Linux') != -1) OSName = 'Linux'

window.onload = () => {
  const downloadButton = document.getElementById('download-game')
  if (OSName === 'MacOS') downloadButton.href = 'game/Hamono_Mac.zip'
  if (OSName === 'Windows') downloadButton.href = 'game/Hamono.exe'
  console.log(downloadButton.href)
}

