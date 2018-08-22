let OSName = 'Unknown OS';
if (navigator.appVersion.indexOf('Win') != -1) OSName = 'Windows'
if (navigator.appVersion.indexOf('Mac') != -1) OSName = 'MacOS'
if (navigator.appVersion.indexOf('X11') != -1) OSName = 'UNIX'
if (navigator.appVersion.indexOf('Linux') != -1) OSName = 'Linux'

window.onload = () => {
  const downloadButton = document.getElementById('download-game')
  const downloadbuttonLarge = document.getElementById('download-game-large')
  if (OSName === 'Unknown OS') { downloadButton.href = '#'; downloadButton.innerText = 'not available'; downloadbuttonLarge.innerText = 'not available' }
  if (OSName !== 'Windows') { downloadButton.href = 'game/Hamono_Mac.zip'; downloadbuttonLarge.href = 'game/Hamono_Mac.zip' }
  // if (OSName === 'Windows') { downloadButton.href = 'game/Hamono.exe'; downloadbuttonLarge.href = 'game/Hamono.zip' }
  if (OSName === 'Windows') { downloadButton.href = '#'; downloadButton.innerText = 'coming soon for windows'; downloadbuttonLarge.innerText = 'coming soon for windows' }

  document.getElementById('story-nav-button').onclick = (event) => {
    document.getElementById('story').scrollIntoView({
      behavior: 'smooth'
    })
  }

  document.getElementById('features-nav-button').onclick = (event) => {
    document.getElementById('features').scrollIntoView({
      behavior: 'smooth'
    })
  }

  document.getElementById('team-nav-button').onclick = (event) => {
    document.getElementById('team').scrollIntoView({
      behavior: 'smooth'
    })
  }
}

