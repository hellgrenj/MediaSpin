var myChart
export function renderBarChart (params) {
  if (myChart != null) {
    // reset previous render
    console.log('clearing previous render')
    myChart.destroy()
  }
  if (params.labels.length > 0 && params.datapoints.length > 0) {
    console.log('params')
    console.log(params) 
    console.log(document.getElementById('bar-chart-horizontal'))
    myChart = new Chart(document.getElementById('bar-chart-horizontal'), {
      type: 'horizontalBar',
      data: {
        labels: params.labels,
        datasets: [
          {
            label: 'antal meningar',
            backgroundColor: [
              '#3e95cd',
              '#8e5ea2',
              '#3cba9f',
              '#e8c3b9',
              '#c45850',
              '#b5c564',
              '#c57f64',
              '#f18fea',
              '#cee8b9',
              '#b9dbe8'
            ],
            data: params.datapoints
          }
        ]
      },
      options: {
        legend: { display: false },
        title: {
          display: true,
          text: 'antal meningar under tidsperiod'
        },
        scales: {
          xAxes: [
            {
              ticks: {
                beginAtZero: true
              }
            }
          ]
        }
      }
    })
  }
}
