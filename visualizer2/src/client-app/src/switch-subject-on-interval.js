var timer = null
export function startRollingSubject (params) {
  if (timer) {
    clearInterval(timer)
  }
  var subjects = params.keywords
  timer = setInterval(function () {
    var subject = subjects[Math.floor(Math.random() * subjects.length)]
    var span = document.getElementById('theSubject')
    if (span) {
      span.innerHTML = subject
    } else {
      console.log('moved of about page so clearing interval')
      clearInterval(timer)
    }
  }, 3000)
}
